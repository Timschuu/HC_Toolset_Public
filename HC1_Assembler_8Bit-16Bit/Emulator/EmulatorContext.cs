using System;
using System.Collections.Generic;
using System.Linq;
using ConsoleArgumentParser;
using HC1_Assembler_8Bit_16Bit.Emulator.Interfaces;
using HC1_Assembler_8Bit_16Bit.Enums;
using HC1_Assembler_8Bit_16Bit.Handler;
using HC1_Assembler_8Bit_16Bit.Helpers.Extensions;
using HC1_Assembler_8Bit_16Bit.Interfaces;
using HC1_Assembler_8Bit_16Bit.Linker;
using HC1_Assembler_8Bit_16Bit.Operations;

namespace HC1_Assembler_8Bit_16Bit.Emulator
{
    public class EmulatorContext
    {
        public int Stepwidth { get; }
        public Parser Parser { get; set; }
        public int ProgramCounter { get; set; }
        
        private readonly Dictionary<int, bool> _breakpoints;
        private readonly int[] _register;
        private readonly int[] _memory;
        private readonly List<byte> _program;
        private readonly int _instructionsize;
        private readonly List<IOperation> _operations;
        private readonly Dictionary<byte, IOperation> _operationsDictionary = new Dictionary<byte, IOperation>();
        private readonly bool _distinctOperations;
        private int _programsize;
        
        public EmulatorContext(int instructionsize, List<byte> program)
        {
            _instructionsize = instructionsize;
            ProgramCounter = 0;
            _register = InitializeRegisters();
            _memory = InitializeMemory();
            _program = program;
            Stepwidth = 1;
            _breakpoints = new Dictionary<int, bool>();
            _operations = OperationRegister.GetOperations(_instructionsize);
            _distinctOperations = CheckOpCodeDistinction();
            if (_distinctOperations)
            {
                _operationsDictionary = CreateOperationDictionary();
            }
            LoadProgramIntoMemory();
        }

        private Dictionary<byte, IOperation> CreateOperationDictionary()
        {
            Dictionary<byte, IOperation> dic = _operations.ToDictionary(o => o.Opcode);
            return dic;
        }
        
        private int[] InitializeRegisters()
        {
            switch (_instructionsize)
            {
                case 8:
                    return new int[1];
                case 16:
                    return new int[32];
                default:
                    throw new NotImplementedException();
            }
        }

        private bool CheckOpCodeDistinction() => _operations.Distinct().Count() == _operations.Count;


        private int[] InitializeMemory()
        {
            switch (_instructionsize)
            {
                case 8:
                    return new int[256];
                case 16:
                    return new int[65536];
                default:
                    throw new NotImplementedException();
            }
        }

        private void LoadProgramIntoMemory()
        {
            try
            {
                switch (_instructionsize)
                {
                    case 8:
                        for (int i = 0; i < _program.Count; i++)
                        {
                            _memory[i] = _program[i];
                        }

                        _programsize = _program.Count;
                        break;
                    case 16:
                        int j = 0;
                        for (int i = 0; i < _program.Count; i += 2)
                        {
                            _memory[j] = _program.ReadTwoBytesReversed(i);
                            j++;
                        }

                        _programsize = _program.Count / 2;
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
            catch (Exception)
            {
                ExceptionHandler.ThrowInvalidMemoryOperationException(0);
            }
        }

        private int GetInstructionFromMemory()
        {
            try
            {
                return _memory[ProgramCounter];
            }
            catch (Exception)
            {
                ExceptionHandler.ThrowInvalidMemoryOperationException(ProgramCounter);
                throw;
            }
        }
        
        public bool EmulateNext(bool output)
        {
            if (ProgramCounter > _programsize)
            {
                Console.WriteLine("The Program ended.");
                return false;
            }

            if (_breakpoints.ContainsKey(ProgramCounter) && _breakpoints[ProgramCounter] != true)
            {
                _breakpoints[ProgramCounter] = true;
                Console.WriteLine("Breakpoint hit.");
                return false;
            }
            
            foreach (var key in _breakpoints.Keys)
            {
                _breakpoints[key] = false;
            }
            
            int instruction = GetInstructionFromMemory();
            IOperation operation = GetOperationFromInstruction(instruction);
            if (!(operation is IEmulatable emulatable))
            {
                throw new NotImplementedException();
            }

            int[] arguments = emulatable.GetArgumentsFromInstruction(instruction);
            emulatable.Emulate(ref Emulator.GetEmulatorContext(), arguments);
            ProgramCounter += Stepwidth;

            if (output)
            {
                Console.WriteLine($"Executed instruction {operation.Mnemonic} with arguments {EmulatorHelper.GetArgumentString(arguments)}");
            }

            return true;
        }

        private IOperation GetOperationFromInstruction(int instruction)
        {
            byte opcode = (byte)LinkerHelper.GetInstructionOpCode(instruction, _instructionsize);
            
            if (_distinctOperations && _operationsDictionary.ContainsKey(opcode))
            {
                return _operationsDictionary[opcode];
            }
            
            List<IOperation> foundOperations = _operations.Where(o => o.Opcode == opcode).ToList();

            if (foundOperations.Count <= 1)
            {
                return foundOperations.FirstOrDefault();
            }
            
            int decidingParameter = instruction & 0x1F;
            IOperation operation;
            switch (decidingParameter)
            {
                case 0:
                case 1:
                    operation = foundOperations.FirstOrDefault(o => o is OpIo opIo && opIo.DataSubst == decidingParameter);
                    break;
                default:
                    operation = foundOperations.FirstOrDefault(o => !(o is OpIo));
                    break;
            }

            if (operation == null)
            {
                throw new InvalidOperationException();
            }

            return operation;
        }

        public void WriteRegisterValuesToConsole()
        {
            Console.WriteLine("Register values:");
            string buf = "[";
            for (int i = 0; i < _register.Length; i++)
            {
                buf += (Register) i + ":" + _register[i] + " ";
            }

            buf = buf.Trim();
            buf += "]";
            Console.WriteLine(buf);
        }

        public void WriteMemoryValuesToConsole(int from, int to)
        {
            Console.WriteLine($"Memory values from address {from} to {to}.");
            string buf = "[";
            for (int i = from; i < to + 1; i++)
            {
                buf += i + ":" + GetMemoryValue(i) + " ";
            }

            buf = buf.Trim();
            buf += "]";
            Console.WriteLine(buf);
        }

        public void WriteEmulationInformationToConsole()
        {
            Console.WriteLine($"Current programcounter: {ProgramCounter}.");
            WriteRegisterValuesToConsole();
        }

        public bool AddBreakpoint(int programcounter)
        {
            if (_breakpoints.ContainsKey(programcounter))
            {
                return false;
            }
            _breakpoints.Add(programcounter, false);
            return true;
        }

        public bool RemoveBreakpoint(int programcounter)
        {
            if (!_breakpoints.ContainsKey(programcounter))
            {
                return false;
            }

            _breakpoints.Remove(programcounter);
            return true;
        }

        public void ClearBreakpoints()
        {
            _breakpoints.Clear();
        }

        public void WriteBreakpointsToConsole()
        {
            for (int i = 0; i < _breakpoints.Count; i++)
            {
                Console.WriteLine($"Breakpoint {i + 1}: {_breakpoints.ElementAt(i).Key}");
            }
        }

        public void WriteProgramCounterToConsole()
        {
            Console.WriteLine($"The current programcounter is {ProgramCounter}.");
        }

        public int GetRegisterValue(int register)
        {
            try
            {
                return _register[register];
            }
            catch (Exception)
            {
                ExceptionHandler.ThrowInvalidRegisterOperationException(register);
                throw;
            }
        }

        public void SetRegisterValue(int register, int value)
        {
            try
            {
                _register[register] = value;
            }
            catch (Exception)
            {
                ExceptionHandler.ThrowInvalidRegisterOperationException(register);
            }
        }

        public void SetMemoryValue(int address, int value)
        {
            try
            {
                _memory[address] = value;
            }
            catch (Exception)
            {
                ExceptionHandler.ThrowInvalidMemoryOperationException(address);
            }
        }

        public int GetMemoryValue(int address)
        {
            try
            {
                return _memory[address];
            }
            catch (Exception)
            {
                ExceptionHandler.ThrowInvalidMemoryOperationException(address);
                throw;
            }
        }
    }
}