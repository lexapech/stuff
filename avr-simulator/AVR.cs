using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace avr
{
    public enum opcode
    {
        unknown, nop, adc, add, and, cp, cpc, cpse, mov, mul, or, sub, sbc, eor,
        asr, com, dec, inc, lsr, neg, pop, push, ror, swap, lpm2, elpm2,
        ijmp, ret, reti, sleep, wdr, lpm, spm, icall, elpm,
        cpi, sbci, subi, ori, andi, ldi, rjmp, rcall,
        movw, muls, cbi, sbic, sbi, sbis, adiw, sbiw,
        mulsu, fmul, fmuls, fmulsu,
        bclr, bset, bld, bst, sbrc, sbrs,
        brbs, brbc, inp, outp, sts, lds,
        jmp, call, ld, st, ldd, std
    }
    public class Instruction
    {
        public opcode opcode;
        public int pc;
        public int arg1;
        public int arg2;
        int arg1index;
        int arg2index;
        public int rawcode;
        public int rawcode2;
        public Instruction(opcode opcode, Tuple<int, int> args)
        {
            this.opcode = opcode;
            arg1index = args.Item1;
            arg2index = args.Item2;
        } 
        
        public Instruction copy()
        {
            return (Instruction)this.MemberwiseClone();
        }


        public void FetchArgs()
        {
            int[] Rd = {(rawcode >> 4) & 0x1F,
                        (rawcode >> 4) & 0x03,
                        rawcode & 0x0FFF,
                        (rawcode >> 4) & 0x07,
                        (rawcode >> 3) & 0x7F,
                        (rawcode >> 4) & 0x0F,
                        (rawcode >> 3) & 0x1F,
                        ((rawcode & 0x1F1) << 16) | rawcode2
                        };
            int[] Rr = { ((rawcode >> 5) & 0x10) | (rawcode & 0x0F),
                        rawcode & 0x07,
                        ((rawcode >> 2)& 0x30)  | (rawcode & 0x0F),
                        (rawcode >> 5) & 0x30 | (rawcode & 0x0F),
                        rawcode& 0x0F,
                        (rawcode >> 4) & 0xF0 | (rawcode & 0x0F),
                        rawcode2
                        };

            if (arg1index != -1)
                arg1 = Rd[arg1index];
            if (arg2index != -1)
                arg2 = Rr[arg2index];
        }

    }



    public class AVR
    {
        public  ushort[] rom = new ushort[4096];
        public byte[] gpr = new byte[32];
        public byte[] io = new byte[64];
        public byte[] ram = new byte[1120];
        public int PC = 0;
        //private enum Flags {C=1,Z=2,N=4,V=8,S=16,H=32,T=64,I=128};



        public void Execute(Instruction ins)
        {
            switch (ins.opcode) {
                case opcode.adc    :ADC   ((byte)ins.arg1, (byte)ins.arg2); break;
                case opcode.add    :ADD   ((byte)ins.arg1, (byte)ins.arg2); break;
                case opcode.and    :AND   ((byte)ins.arg1, (byte)ins.arg2); break;
                case opcode.cp     :CP    ((byte)ins.arg1, (byte)ins.arg2); break;
                case opcode.cpc    :CPC   ((byte)ins.arg1, (byte)ins.arg2); break;
                case opcode.cpse   :CPSE  ((byte)ins.arg1, (byte)ins.arg2); break;
                case opcode.mov    :MOV   ((byte)ins.arg1, (byte)ins.arg2); break;
                case opcode.mul    :MUL   ((byte)ins.arg1, (byte)ins.arg2); break;
                case opcode.or     :OR    ((byte)ins.arg1, (byte)ins.arg2); break;
                case opcode.sub    :SUB   ((byte)ins.arg1, (byte)ins.arg2); break;
                case opcode.sbc    :SBC   ((byte)ins.arg1, (byte)ins.arg2); break;
                case opcode.eor    :EOR   ((byte)ins.arg1, (byte)ins.arg2); break;

                case opcode.brbs   :BRBS((byte)ins.arg1, (byte)ins.arg2); break;
                case opcode.brbc   :BRBC((byte)ins.arg1, (byte)ins.arg2); break;

                case opcode.adiw   :ADIW((byte)ins.arg1, (byte)ins.arg2); break;
                case opcode.sbiw   :SBIW((byte)ins.arg1, (byte)ins.arg2); break;

                case opcode.rjmp   :RJMP  ((ushort)ins.arg1); break;
                case opcode.rcall  :RCALL ((ushort)ins.arg1); break;

                case opcode.asr    :ASR   ((byte)ins.arg1); break;
                case opcode.com    :COM   ((byte)ins.arg1); break;
                case opcode.dec    :DEC   ((byte)ins.arg1); break;
                case opcode.inc    :INC   ((byte)ins.arg1); break;
                case opcode.lsr    :LSR   ((byte)ins.arg1); break;
                case opcode.neg    :NEG   ((byte)ins.arg1); break;
                case opcode.pop    :POP   ((byte)ins.arg1); break;
                case opcode.push   :PUSH  ((byte)ins.arg1); break;
                case opcode.ror    :ROR   ((byte)ins.arg1); break;
                case opcode.swap   :SWAP  ((byte)ins.arg1); break;
                case opcode.lpm2   :LPM2  ((byte)ins.arg1); break;
                case opcode.elpm2  :ELPM2 ((byte)ins.arg1); break;

                case opcode.bclr   :BCLR  ((byte)ins.arg1); break;
                case opcode.bset   :BSET  ((byte)ins.arg1); break;
                case opcode.st     :ST    ((byte)ins.arg1, (ushort)(ins.rawcode & 0x3C0F)); break;
                case opcode.ld     :LD    ((byte)ins.arg1, (ushort)(ins.rawcode & 0x3C0F)); break;
                case opcode.std    :ST    ((byte)ins.arg1, (ushort)(ins.rawcode & 0x3C0F)); break;
                case opcode.ldd    :LD    ((byte)ins.arg1, (ushort)(ins.rawcode & 0x3C0F)); break;

                case opcode.bld    :BLD   ((byte)ins.arg1, (byte)ins.arg2); break;
                case opcode.bst    :BST   ((byte)ins.arg1, (byte)ins.arg2); break;
                case opcode.sbrc   :SBRC  ((byte)ins.arg1, (byte)ins.arg2); break;
                case opcode.sbrs   :SBRS  ((byte)ins.arg1, (byte)ins.arg2); break;

                case opcode.nop    :NOP   (); break;
                case opcode.ijmp   :IJMP  (); break;
                case opcode.ret    :RET   (); break;
                case opcode.reti   :RETI  (); break;
                case opcode.sleep  :SLEEP (); break;
                case opcode.wdr    :WDR   (); break;
                case opcode.lpm    :LPM   (); break;
                case opcode.spm    :SPM   (); break;
                case opcode.icall  :ICALL (); break;
                case opcode.elpm   :ELPM  (); break;

                case opcode.inp    :IN   ((byte)ins.arg1, (byte)ins.arg2); break;
                case opcode.outp   :OUT  ((byte)ins.arg2, (byte)ins.arg1); break;

                case opcode.mulsu  :MULSU ((byte)ins.arg1, (byte)ins.arg2); break;
                case opcode.fmul   :FMUL  ((byte)ins.arg1, (byte)ins.arg2); break;
                case opcode.fmuls  :FMULS ((byte)ins.arg1, (byte)ins.arg2); break;
                case opcode.fmulsu :FMULSU((byte)ins.arg1, (byte)ins.arg2); break;

                case opcode.movw   :MOVW  ((byte)ins.arg1, (byte)ins.arg2); break;
                case opcode.muls   :MULS  ((byte)ins.arg1, (byte)ins.arg2); break;

                case opcode.cpi    :CPI   ((byte)ins.arg1, (byte)ins.arg2); break;
                case opcode.sbci   :SBCI  ((byte)ins.arg1, (byte)ins.arg2); break;
                case opcode.subi   :SUBI  ((byte)ins.arg1, (byte)ins.arg2); break;
                case opcode.ori    :ORI   ((byte)ins.arg1, (byte)ins.arg2); break;
                case opcode.andi   :ANDI  ((byte)ins.arg1, (byte)ins.arg2); break;
                case opcode.ldi    :LDI   ((byte)ins.arg1, (byte)ins.arg2); break;

                case opcode.cbi    :CBI   ((byte)ins.arg1, (byte)ins.arg2); break;
                case opcode.sbic   :SBIC  ((byte)ins.arg1, (byte)ins.arg2); break;
                case opcode.sbi    :SBI   ((byte)ins.arg1, (byte)ins.arg2); break;
                case opcode.sbis   :SBIS  ((byte)ins.arg1, (byte)ins.arg2); break;
                    
                case opcode.jmp    :JMP   (ins.arg1); break;
                case opcode.call   :CALL  (ins.arg1); break;
                case opcode.sts    :STS   (ins.arg2, (byte)ins.arg1); break;
                case opcode.lds    :LDS   (ins.arg2, (byte)ins.arg1); break;
            }
        }


        static readonly Dictionary<int, Instruction> group1 = new Dictionary<int, Instruction>
            {
                {0x1C00,new Instruction( opcode.adc,new Tuple<int,int>(0,0))},
                {0x0C00, new Instruction(opcode.add ,new Tuple<int,int>(0,0))},
                {0x2000, new Instruction(opcode.and ,new Tuple<int,int>(0,0))},
                {0x1400, new Instruction(opcode.cp  ,new Tuple<int,int>(0,0))},
                {0x0400, new Instruction(opcode.cpc ,new Tuple<int,int>(0,0))},
                {0x1000, new Instruction(opcode.cpse,new Tuple<int,int>(0,0))},
                {0x2C00, new Instruction(opcode.mov ,new Tuple<int,int>(0,0))},
                {0x9C00, new Instruction(opcode.mul ,new Tuple<int,int>(0,0))},
                {0x2800, new Instruction(opcode.or  ,new Tuple<int,int>(0,0))},
                {0x1800, new Instruction(opcode.sub ,new Tuple<int,int>(0,0))},
                {0x0800, new Instruction(opcode.sbc ,new Tuple<int,int>(0,0))},
                {0x2400, new Instruction(opcode.eor ,new Tuple<int,int>(0,0))},

                {0xF000, new Instruction(opcode.brbs,new Tuple<int,int>(4,1))},
                {0xF400, new Instruction(opcode.brbc,new Tuple<int,int>(4,1))}
            };
        static readonly Dictionary<int, Instruction> group2 = new Dictionary<int, Instruction>
            {
                {0x9405, new Instruction(opcode.asr  ,new Tuple<int,int>(0,-1))},
                {0x9400, new Instruction(opcode.com  ,new Tuple<int,int>(0,-1))},
                {0x940A, new Instruction(opcode.dec  ,new Tuple<int,int>(0,-1))},
                {0x9403, new Instruction(opcode.inc  ,new Tuple<int,int>(0,-1))},
                {0x9406, new Instruction(opcode.lsr  ,new Tuple<int,int>(0,-1))},
                {0x9401, new Instruction(opcode.neg  ,new Tuple<int,int>(0,-1))},
                {0x900F, new Instruction(opcode.pop  ,new Tuple<int,int>(0,-1))},
                {0x920F, new Instruction(opcode.push ,new Tuple<int,int>(0,-1))},
                {0x9407, new Instruction(opcode.ror  ,new Tuple<int,int>(0,-1))},
                {0x9402, new Instruction(opcode.swap ,new Tuple<int,int>(0,-1))},
                {0x9004, new Instruction(opcode.lpm ,new Tuple<int,int>(0,-1))},
                {0x9005, new Instruction(opcode.lpm2 ,new Tuple<int,int>(0,-1))},
                {0x9006, new Instruction(opcode.elpm2,new Tuple<int,int>(0,-1))},
                {0x9200, new Instruction(opcode.sts  ,new Tuple<int,int>(0,6))},
                {0x9000, new Instruction(opcode.lds  ,new Tuple<int,int>(0,6))}
            };
        static readonly Dictionary<int, Instruction> group3 = new Dictionary<int, Instruction>
            {
                {0x0000, new Instruction(opcode.nop  ,new Tuple<int,int>(-1,-1))},
                {0x9409, new Instruction(opcode.ijmp ,new Tuple<int,int>(-1,-1))},
                {0x9508, new Instruction(opcode.ret  ,new Tuple<int,int>(-1,-1))},
                {0x9518, new Instruction(opcode.reti ,new Tuple<int,int>(-1,-1))},
                {0x9588, new Instruction(opcode.sleep,new Tuple<int,int>(-1,-1))},
                {0x95A8, new Instruction(opcode.wdr  ,new Tuple<int,int>(-1,-1))},
                {0x95C8, new Instruction(opcode.lpm  ,new Tuple<int,int>(-1,-1))},
                {0x95E8, new Instruction(opcode.spm  ,new Tuple<int,int>(-1,-1))},
                {0x9509, new Instruction(opcode.icall,new Tuple<int,int>(-1,-1))},
                {0x95D8, new Instruction(opcode.elpm ,new Tuple<int,int>(-1,-1))},
            };
 
        static readonly Dictionary<int, Instruction> group4 = new Dictionary<int, Instruction>
            {
                {0x3000, new Instruction(opcode.cpi ,new Tuple<int,int>(5,5))},
                {0x4000, new Instruction(opcode.sbci,new Tuple<int,int>(5,5))},
                {0x5000, new Instruction(opcode.subi,new Tuple<int,int>(5,5))},
                {0x6000, new Instruction(opcode.ori ,new Tuple<int,int>(5,5))},
                {0x7000, new Instruction(opcode.andi,new Tuple<int,int>(5,5))},
                {0xE000, new Instruction(opcode.ldi ,new Tuple<int,int>(5,5))},

                {0xC000, new Instruction(opcode.rjmp ,new Tuple<int,int>(2,-1))},
                {0xD000, new Instruction(opcode.rcall,new Tuple<int,int>(2,-1))},
            };

        static readonly Dictionary<int, Instruction> group5 = new Dictionary<int, Instruction>
            {
                {0x0100, new Instruction(opcode.movw,new Tuple<int,int>(5 , 4))},
                {0x0200, new Instruction(opcode.muls,new Tuple<int,int>(5 , 4))},
                {0x9800, new Instruction(opcode.cbi ,new Tuple<int,int>(6,1))},
                {0x9900, new Instruction(opcode.sbic,new Tuple<int,int>(6,1))},
                {0x9A00, new Instruction(opcode.sbi ,new Tuple<int,int>(6,1))},
                {0x9B00, new Instruction(opcode.sbis,new Tuple<int,int>(6,1))},
                {0x9600, new Instruction(opcode.adiw,new Tuple<int,int>(1,2))},
                {0x9700, new Instruction(opcode.sbiw,new Tuple<int,int>(1,2))},

            };
        static readonly Dictionary<int, Instruction> group6 = new Dictionary<int, Instruction>
            {
                {0x0300, new Instruction(opcode.mulsu ,new Tuple<int,int>(3 , 1))},
                {0x0308, new Instruction(opcode.fmul  ,new Tuple<int,int>(3 , 1))},
                {0x0380, new Instruction(opcode.fmuls ,new Tuple<int,int>(3 , 1))},
                {0x0388, new Instruction(opcode.fmulsu,new Tuple<int,int>(3 , 1))},
            };
        static readonly Dictionary<int, Instruction> group7 = new Dictionary<int, Instruction>
            {
                {0x9488, new Instruction(opcode.bclr,new Tuple<int,int>(3,-1))},
                {0x9408, new Instruction(opcode.bset,new Tuple<int,int>(3,-1))},
            };
        static readonly Dictionary<int, Instruction> group8 = new Dictionary<int, Instruction>
            {
                {0xF800,new Instruction( opcode.bld ,new Tuple<int,int>(0,1))},
                {0xFA00,new Instruction( opcode.bst ,new Tuple<int,int>(0,1))},
                {0xFC00,new Instruction( opcode.sbrc,new Tuple<int,int>(0,1))},
                {0xFE00,new Instruction( opcode.sbrs,new Tuple<int,int>(0,1))},
            };
        static readonly Dictionary<int, Instruction> group9 = new Dictionary<int, Instruction>
            {
                {0xB000, new Instruction(opcode.inp,new Tuple<int,int>(0 ,3 ))},
                {0xB800, new Instruction(opcode.outp,new Tuple<int,int>(0 ,3 ))},
            };
        static readonly Dictionary<int, Instruction> group10 = new Dictionary<int, Instruction>
            {
                {0x940C, new Instruction(opcode.jmp,new Tuple<int,int>(7,-1))},
                {0x940E, new Instruction(opcode.call,new Tuple<int,int>(7,-1))},
            };
        bool group11(int cmd,out Instruction res)
        {
            if ((cmd & 0xD000) == 0x9000)
            {
                switch (cmd & 0xFE00)
                {
                    case 0x9000:
                        res = new Instruction(opcode.ld, new Tuple<int, int>(0, -1));
                        return true;
                    case 0x9200:
                        res = new Instruction(opcode.st, new Tuple<int, int>(0, -1));
                        return true;
                    default:
                        res = new Instruction(opcode.unknown, new Tuple<int, int>(-1, -1));
                        return false;
                }
            }
            else if ((cmd & 0xD000) == 0x8000)
            {
                switch (cmd & 0xD200)
                {
                    case 0x8000:
                        res = new Instruction(opcode.ldd, new Tuple<int, int>(0, -1));
                        return true;
                    case 0x8200:
                        res = new Instruction(opcode.std, new Tuple<int, int>(0, -1));
                        return true;
                    default:
                        res = new Instruction(opcode.unknown, new Tuple<int, int>(-1, -1));
                        return false;
                }
            }
            else res = new Instruction(opcode.unknown, new Tuple<int, int>(-1, -1)); return false;
        }

        static readonly int[] masks = {0xFC00,0xFE0F,0xFFFF,0xF000,0xFF00,0xFFC8,0xFF8F,0xFE08,0xF800,0xFE0E};
        static readonly Dictionary<int, Instruction>[] groups = { group1, group2, group3, group4, group5, group6, group7, group8, group9, group10 };
        public Instruction Decode(int pc)
        {        
                int cmd = rom[pc];


                Instruction instr = new Instruction(opcode.unknown, new Tuple<int, int>(-1, -1));

                bool found = false;

                for (int i = 0; i < groups.Length; i++)
                {
                    found = groups[i].TryGetValue(cmd & masks[i], out instr);
                    if (found) break;

                }
                if (!found) found = group11(cmd, out instr);

                
                instr.rawcode = cmd;
                if (pc!=4095)
                    instr.rawcode2 = rom[pc + 1];
                instr.pc = pc;
                if (instr.opcode == opcode.unknown) return new Instruction(opcode.unknown, new Tuple<int, int>(-1, -1));
                instr.FetchArgs();
                return instr;
        }




        private int C
        {
            get
            {
                return io[0x3F] & 0x01;
            }
            set
            {
                if (value != 0)
                    io[0x3F] |= 0x01;
                else
                    io[0x3F] &= (~0x01) & 0xFF;
            }
        }
        private int Z
        {
            get {
                return (io[0x3F] & 0x02) >> 1;
            }
            set {
                if (value != 0)
                    io[0x3F] |= 0x02;
                else
                    io[0x3F] &= (~0x02) & 0xFF;
            }
        }
        private int N
        {
            get
            {
                return (io[0x3F] & 0x04) >> 2;
            }
            set
            {
                if (value != 0)
                    io[0x3F] |= 0x04;
                else
                    io[0x3F] &= (~0x04) & 0xFF;
            }
        }
        private int V
        {
            get
            {
                return (io[0x3F] & 0x08) >> 3;
            }
            set
            {
                if (value != 0)
                    io[0x3F] |= 0x08;
                else
                    io[0x3F] &= (~0x08) & 0xFF;
            }
        }
        private int S
        {
            get
            {
                return (io[0x3F] & 0x10) >> 4;
            }
            set
            {
                if (value != 0)
                    io[0x3F] |= 0x10;
                else
                    io[0x3F] &= (~0x10) & 0xFF;
            }
        }
        private int H
        {
            get
            {
                return (io[0x3F] & 0x20) >> 5;
            }
            set
            {
                if (value != 0)
                    io[0x3F] |= 0x20;
                else
                    io[0x3F] &= (~0x20) & 0xFF;
            }
        }
        private int T
        {
            get
            {
                return (io[0x3F] & 0x40) >> 6;
            }
            set
            {
                if (value != 0)
                    io[0x3F] |= 0x40;
                else
                    io[0x3F] &= (~0x40) & 0xFF;
            }
        }
        private int I
        {
            get
            {
                return (io[0x3F] & 0x80) >> 7;
            }
            set
            {
                if (value != 0)
                    io[0x3F] |= 0x80;
                else
                    io[0x3F] &= (~0x80) & 0xFF;
            }
        }
        private bool asleep = false;

        private byte Low(byte b)
        {
            return (byte)(b & 0x0F);
        }

        public void ADC(byte d, byte r)
        {
            byte Rd = gpr[d];
            byte Rr = gpr[r];
            int res = Rd + Rr + C;
              
            H = (Low(Rd) + Low(Rr) + C) >> 4;
            V = ((Rd & Rr & ~res) | (~Rd & ~Rr & res)) & 0x80;
            N = res & 0x80;
            Z = res == 0 ? 1 : 0;
            C = res >> 8;
            S = N ^ V;

            gpr[d] = (byte)(res & 0xFF);
            PC++;
            }
            public void ADD(byte d, byte r)
            {
                byte Rd = gpr[d];
                byte Rr = gpr[r];

                int res = Rd + Rr;

                H = (Low(Rd) + Low(Rr)) >> 4;
                V = ((Rd & Rr & ~res) | (~Rd & ~Rr & res)) & 0x80;
                N = res & 0x80;
                Z = res == 0 ? 1 : 0;
                C = res >> 8;
                S = N ^ V;
                gpr[d] = (byte)(res & 0xFF);
                PC++;
            }
            public void AND(byte d, byte r)
            {
                byte Rd = gpr[d];
                byte Rr = gpr[r];

                int res = Rd & Rr;

                N = res & 0x80;
                Z = res == 0 ? 1 : 0;
                V = 0;
                S = N ^ V;
                gpr[d] = (byte)(res & 0xFF);
                PC++;
            }
            public void CP(byte d, byte r)
            {
                byte Rd = gpr[d];
                byte Rr = gpr[r];

                int res = Rd - Rr;

                H = ~(0x10 + Low(Rd) - Low(Rr)) >> 4;
                V = ((Rd & ~Rr & ~res) | (~Rd & Rr & res)) & 0x80;
                N = res & 0x80;
                Z = res == 0 ? 1 : 0;
                C = res >> 8;
                S = N ^ V;
                PC++;
            }
            public void CPC(byte d, byte r)
            {
                byte Rd = gpr[d];
                byte Rr = gpr[r];

                int res = Rd - Rr-C;

                H = ~(0x10 + Low(Rd) - Low(Rr) - C) >> 4;
                V = ((Rd & ~Rr & ~res) | (~Rd & Rr & res)) & 0x80;
                N = res & 0x80;
                Z = res == 0 ? 1 : 0;
                C = res >> 8;
                S = N ^ V;
                PC++;
            }
            public void CPSE(byte d, byte r)
            {
                byte Rd = gpr[d];
                byte Rr = gpr[r];

                if (Rd == Rr)
                {
                ushort nextcmd = rom[PC + 1];
                    if ((nextcmd & 0xFE0E) == 0x940C
                        | (nextcmd & 0xFE0E) == 0x940E
                        | (nextcmd & 0xFE0F) == 0x9200
                        | (nextcmd & 0xFE0F) == 0x9000
                    ) PC += 3;
                }
                else PC++;

            }
            public void MOV(byte d, byte r)
            {

                gpr[d] = gpr[r];

                PC++;
            }
            public void MUL(byte d, byte r)
            {
                byte Rd = gpr[d];
                byte Rr = gpr[r];

                int tmp = Rd * Rr;
                C = tmp & 0x8000;

                gpr[1] = (byte)((tmp & 0xFF00) >> 8);
                gpr[0] = (byte)(tmp & 0xFF);
                PC++;
            }
            public void OR(byte d, byte r)
            {
                byte Rd = gpr[d];
                byte Rr = gpr[r];

                int res = Rd | Rr;
          
                N = res & 0x80;
                Z = res == 0 ? 1 : 0;
                V = 0;
                S = N ^ V;
                gpr[d] = (byte)(res & 0xFF);
                PC++;
            }
            public void SBC(byte d, byte r)
            {
                byte Rd = gpr[d];
                byte Rr = gpr[r];

                int res = Rd - Rr - C;

                H = ~(0x10 + Low(Rd) - Low(Rr) - C) >> 4;
                V = ((Rd & ~Rr & ~res) | (~Rd & Rr & res)) & 0x80;
                N = res & 0x80;
                Z = res == 0 ? 1 : 0;
                C = res >> 8;
                S = N ^ V;
                PC++;
                gpr[d] = (byte)(res & 0xFF);
            }
            public void SUB(byte d, byte r)
            {
                byte Rd = gpr[d];
                byte Rr = gpr[r];

                int res = Rd - Rr;

                H = ~(0x10 + Low(Rd) - Low(Rr)) >> 4;

                V = ((Rd & ~Rr & ~res) | (~Rd & Rr & res)) & 0x80;
                //V = (Rd ^ (Rr | res)) & 0x80;
                N = res & 0x80;
                Z = res == 0 ? 1 : 0;
                C = res >> 8;
                S = N ^ V;
                PC++;
                gpr[d] = (byte)(res & 0xFF);
            }
            public void EOR(byte d, byte r)
            {
                byte Rd = gpr[d];
                byte Rr = gpr[r];

                int res = Rd ^ Rr;
                N = res & 0x80;
                Z = res == 0 ? 1 : 0;
                V = 0;
                S = N ^ V;
                gpr[d] = (byte)(res & 0xFF);
                PC++;
            }
            public void ADIW(byte d, byte r)
            {
                byte Rdl = gpr[d + 24];
                byte Rdh = gpr[d + 25];
                int res = (Rdh << 8) + Rdl + r;

                N = res & 0x8000;

                C = res >> 16;
                Z = res == 0 ? 1 : 0;
                V = (~Rdh & (res>>8)) & 0x80;
                S = N ^ V;
                gpr[d + 24] = (byte)(res & 0xFF);
                gpr[d + 25] = (byte)((res & 0xFF00) >> 8);
                PC++;
            }
            public void SBIW(byte d, byte r)
            {
                byte Rdl = gpr[d + 24];
                byte Rdh = gpr[d + 25];
                int res = (Rdh << 8) + Rdl - r;
                N = res & 0x8000;
                C = res >> 16;
                Z = res == 0 ? 1 : 0;
                V = (~Rdh & (res >> 8)) & 0x80;
                S = N ^ V;
                gpr[d + 24] = (byte)(res & 0xFF);
                gpr[d + 25] = (byte)((res & 0xFF00) >> 8);
                PC++;
            }
            public void RJMP(ushort adr)
            {
            if ((adr & 0x800) == 0) PC = PC + adr + 1;
            else PC = PC - 4095 + adr;
            }
            public void RCALL(ushort adr)
            {
                int SP = ((ushort)io[0x3E] << 8) + io[0x3D];
                ram[--SP] = (byte)(PC >> 8);
                ram[--SP] = (byte)PC;
                
                io[0x3E] = (byte)(SP>> 8);
                io[0x3D] = (byte)SP;

                RJMP(adr);
            }
            public void ASR(byte d)
            {
                byte Rd = gpr[d];
                byte res = (byte)((Rd >> 1) | (Rd & 0x80));

                C = Rd & 0x01;
                N = res & 0x80;
                Z = res == 0 ? 1 : 0;
                V = N ^ C;
                S = N ^ V;
                gpr[d] = res;
                PC++;
            }
            /***/
            public void COM(byte d)
            {
                byte Rd = gpr[d];
                byte res = (byte)~Rd;

                C = 1;
                N = res & 0x80;
                Z = res == 0 ? 1 : 0;
                V = 0;
                S = N ^ V;
                gpr[d] = res;
                PC++;
            }
            /***/
            public void DEC(byte d)
            {
                byte Rd = gpr[d];
                V = Rd == 0x80 ? 1 : 0;
                Rd--;
                N = Rd & 0x80;
                Z = Rd == 0 ? 1 : 0;
                S = N ^ V;
                gpr[d] = Rd;
                PC++;
            }
            public void INC(byte d)
            {
                byte Rd = gpr[d];
                V = Rd == 0x7F ? 1 : 0;
                Rd++;
                N = Rd & 0x80;
                Z = Rd == 0 ? 1 : 0;
                S = N ^ V;
                gpr[d] = Rd;
                PC++;
            }
            public void LSR(byte d)
            {

                byte Rd = gpr[d];
                byte res = (byte)(Rd >> 1);

                C = Rd & 0x01;
                N = Rd & 0x80;
                Z = Rd == 0 ? 1 : 0;
                V = N ^ C;
                S = N ^ V;
                gpr[d] = res;
                PC++;
            }
            public void NEG(byte d)
            {
                byte Rd = gpr[d];
                int res = 0-Rd;

                H = ~(0x10 - Low(Rd)) >> 4;
                V = (Rd  ^ res) & 0x80;
                N = res & 0x80;
                Z = res == 0 ? 1 : 0;
                C = res >> 8;
                S = N ^ V;
                PC++;
                gpr[d] = (byte)(res & 0xFF);
            }
            public void POP(byte d)
            {
                int SP = ((ushort)io[0x3E] << 8) + io[0x3D];

                gpr[d] = ram[SP];
                SP++;
                io[0x3E] = (byte)(SP >> 8);
                io[0x3D] = (byte)SP;
                PC++;
            }
            public void PUSH(byte d)
            {
                int SP = ((ushort)io[0x3E] << 8) + io[0x3D];

                SP -= 1;
                ram[SP] = gpr[d];

                io[0x3E] = (byte)((SP & 0xFF00) >> 8);
                io[0x3D] = (byte)(SP & 0xFF);
                PC++;
            }
            public void ROR(byte d)
            {
                byte Rd = gpr[d];
                byte res = (byte)((Rd >> 1) | (C << 7));

                C = Rd & 0x01;
                N = res & 0x80;
                Z = res == 0 ? 1 : 0;
                V = N ^ C;
                S = N ^ V;
                gpr[d] = res;
                PC++;
            }
            public void SWAP(byte d)
            {
                byte Rd = gpr[d];
                byte res= (byte)(Rd << 4 | Rd >> 4 );
                gpr[d]=res;
                PC++;
            }
        //LD  проверить
            public void LD(byte d, ushort r)
            {

                int X = gpr[26] + (gpr[27] << 8);
                int Y = gpr[28] + (gpr[29] << 8);
                int Z = gpr[30] + (gpr[31] << 8);
                int m1 = (r >> 12) & 0x01;
                if (m1 == 1)
                {
                    switch (r)
                    {
                        case 0x100C:
                            gpr[d] = ram[X];
                            break;
                        case 0x100E:
                            X--;
                            gpr[d] = ram[X];
                            gpr[26] = (byte)X;
                            gpr[27] = (byte)(X >> 8);
                            break;
                        case 0x100D:
                            gpr[d] = ram[X];
                            X++;
                            gpr[26] = (byte)X;
                            gpr[27] = (byte)(X >> 8);
                            break;
                        case 0x100A:
                            Y--;
                            gpr[d] = ram[Y];
                            gpr[28] = (byte)Y;
                            gpr[29] = (byte)(Y >> 8);
                            break;
                        case 0x1009:
                            gpr[d] = ram[Y];
                            Y++;
                            gpr[28] = (byte)Y;
                            gpr[29] = (byte)(Y >> 8);
                            break;
                        case 0x1002:
                            Z--;
                            gpr[d] = ram[Z];
                            gpr[30] = (byte)Z;
                            gpr[31] = (byte)(Z >> 8);
                            break;
                        case 0x1001:
                            gpr[d] = ram[Z];
                            Z++;
                            gpr[30] = (byte)Z;
                            gpr[31] = (byte)(Z >> 8);
                            break;
                    }
                }
                else
                {
                    int q = r & 0x07 | (r & 0x0C00) >> 7 | (r & 0x2000) >> 8;
                    switch (r & 0x0008)
                    {
                        case 0x0008:
                            gpr[d] = ram[Y + q];
                            break;

                        case 0x0000:
                            gpr[d] = ram[Z + q];
                            break;


                    }

                }

                
                PC++;
            }
        //ST проверить
            public void ST(byte d, ushort r)
            {

            int X = gpr[26] + (gpr[27] << 8);
            int Y = gpr[28] + (gpr[29] << 8);
            int Z = gpr[30] + (gpr[31] << 8);
            int m1 = (r >> 12) & 0x01;
            if (m1 == 1)
            {
                switch (r)
                {
                    case 0x100C:
                        ram[X] = gpr[d];
                        break;
                    case 0x100E:
                        X--;
                        ram[X] = gpr[d];
                        gpr[26] = (byte)X;
                        gpr[27] = (byte)(X >> 8);
                        break;
                    case 0x100D:
                        ram[X] = gpr[d];
                        X++;
                        gpr[26] = (byte)X;
                        gpr[27] = (byte)(X >> 8);
                        break;
                    case 0x100A:
                        Y--;
                        ram[Y] = gpr[d];
                        gpr[28] = (byte)Y;
                        gpr[29] = (byte)(Y >> 8);
                        break;
                    case 0x1009:
                        ram[Y] = gpr[d];
                        Y++;
                        gpr[28] = (byte)Y;
                        gpr[29] = (byte)(Y >> 8);
                        break;
                    case 0x1002:
                        Z--;
                        ram[Z] = gpr[d];
                        gpr[30] = (byte)Z;
                        gpr[31] = (byte)(Z >> 8);
                        break;
                    case 0x1001:
                        ram[Z] = gpr[d];
                        Z++;
                        gpr[30] = (byte)Z;
                        gpr[31] = (byte)(Z >> 8);
                        break;
                }
            }
            else
            {
                int q = r & 0x07 | (r & 0x0C00) >> 7 | (r & 0x2000) >> 8;
                switch (r & 0x0008)
                {
                    case 0x0008:
                        ram[Y + q]= gpr[d];
                        break;

                    case 0x0000:
                        ram[Z + q]= gpr[d];
                        break;


                }

            }

            PC++;
        }
            public void LPM(byte d = 0)
            {
                ushort addr = (ushort)(gpr[30] + (gpr[31] << 8));
                gpr[0] = (byte)(rom[addr >> 1] >> ((addr & 0x01) * 8));
                PC++;
            }
            public void LPM2(byte d = 0)
            {
                ushort addr = (ushort)(gpr[30] + (gpr[31] << 8));
                gpr[0] = (byte)((rom[addr >> 1] >> (addr & 1) * 8) & 0xFF);
                addr++;
                gpr[30] = (byte)(addr & 0xFF);
                gpr[31] = (byte)(addr >> 8);
                PC++;
            }
            /**/
            public void ELPM(byte d = 0)
            {

                PC++;
            }
            public void ELPM2(byte d)
            {

                PC++;
            }
            /***/
            public void BCLR(byte d)
            {
                io[0x3F] &= (byte)~(1 << d);
                PC++;
            }
            public void BSET(byte d)
            {
                io[0x3F] |= (byte)(1 << d);
                PC++;
            }
            public void BLD(byte d, byte r)
            {
                byte Rd = gpr[d];

                if (T!=0) Rd |= (byte)(1 << r);
                else Rd &= (byte)~(1 << r);
                PC++;
            }
            public void BST(byte d, byte r)
            {
                T = gpr[d] & (1 << r);              
                PC++;
            }
            public void SBRC(byte d, byte r)
            {
                byte Rd = gpr[d];
                
                if ((Rd & (1 << r)) == 0)
                {
                    ushort next = rom[PC + 1];
                    if ((next & 0xFE0E) == 0x940C
                        | (next & 0xFE0E) == 0x940E
                        | (next & 0xFE0F) == 0x9200
                        | (next & 0xFE0F) == 0x9000
                        ) PC += 3;
                    else PC += 2;

                }
                else PC++;
            }
            public void SBRS(byte d, byte r)
            {
                byte Rd = gpr[d];
                if ((Rd & (1 << r)) > 0)
                {
                    ushort next = rom[PC + 1];
                    if ((next & 0xFE0E) == 0x940C
                        | (next & 0xFE0E) == 0x940E
                        | (next & 0xFE0F) == 0x9200
                        | (next & 0xFE0F) == 0x9000
                        ) PC += 3;
                    else PC += 2;
                }
                else PC++;
            }
            public void BRBC(byte adr, byte r)
            {
                byte Rd = io[0x3F];
                if ((Rd & (1 << r)) == 0)
                {
                    if (adr < 0x40) PC = PC + adr + 1;
                    else PC = PC - 127 + adr;

                }
                else PC++;
            }
            public void BRBS(byte adr, byte r)
            {
                byte Rd = io[0x3F];
                if ((Rd & (1 << r)) > 0)
                {
                    if (adr < 0x40) PC = PC + adr + 1;
                    else PC = PC - 127 + adr;

                }
                else PC++;
            }
            public void NOP()
            {
                PC++;
            }
            public void IJMP()
            {
                PC = (ushort)(gpr[30] + (gpr[31] << 8));
            }
            public void RET()
            {
                int SP = ((ushort)io[0x3E] << 8) + io[0x3D];
                PC = (ushort)(ram[SP] + (ram[SP + 1] << 8));
                SP += 2;
                io[0x3E] = (byte)(SP >> 8);
                io[0x3D] = (byte)SP;
                 PC++;           
            }
            public void RETI()
            {
                int SP = ((ushort)io[0x3E] << 8) + io[0x3D];

                PC = (ushort)(ram[SP] + (ram[SP + 1] << 8));
                SP += 2;
                io[0x3E] = (byte)(SP >> 8);
                io[0x3D] = (byte)SP;
                PC++;
                io[0x3F] |= 128;
            }
            public void SLEEP()
            {
                asleep = true;
                PC++;
            }
            public void WDR()
            {
                PC++;
            }
            public void SPM()
            {
                PC++;
            }
            public void ICALL()
            {
                int SP = ((ushort)io[0x3E] << 8) + io[0x3D];
                ram[--SP] = (byte)((PC & 0xFF00) >> 8);
                ram[--SP] = (byte)(PC & 0xFF);
                

                io[0x3E] = (byte)((SP & 0xFF00) >> 8);
                io[0x3D] = (byte)(SP & 0xFF);

                PC = (ushort)(gpr[30] + (gpr[31] << 8));

            }
            public void IN(byte d, byte r)
            {
                gpr[d] = io[r];
                PC++;
            }
            public void OUT(byte d, byte r)
            {
                io[d] = gpr[r];
                PC++;
            }
            /************************/
            public void FMULSU(byte d, byte r)
            {
                ushort tmp = (ushort)(gpr[d + 0x10] * gpr[r + 0x10]);
                C = tmp & 0x8000;
                Z = tmp == 0 ? 1 : 0;
                tmp = (ushort)(tmp << 1);
                gpr[0] = (byte)tmp;
                gpr[1] = (byte)(tmp >> 8);
                PC++;
            }
            public void FMUL(byte d, byte r)
            {
                ushort tmp = (ushort)(gpr[d + 0x10] * gpr[r + 0x10]);
                C = tmp & 0x8000;
                Z = tmp == 0 ? 1 : 0;
                tmp = (ushort)(tmp << 1);
                gpr[0] = (byte)tmp;
                gpr[1] = (byte)(tmp >> 8);
                PC++;
            }
            public void FMULS(byte d, byte r)
            {
                ushort tmp = (ushort)(gpr[d + 0x10] * gpr[r + 0x10]);
                C = tmp & 0x8000;
                Z = tmp == 0 ? 1 : 0;
                tmp = (ushort)(tmp << 1);
                gpr[0] = (byte)(tmp & 0xFF);
                gpr[1] = (byte)(tmp >> 8);
                PC++;
            }
            public void MULSU(byte d, byte r)
            {
                ushort tmp = (ushort)(gpr[d + 0x10] * gpr[r + 0x10]);
                C = tmp & 0x8000;
                Z = tmp == 0 ? 1 : 0;
                tmp = (ushort)(tmp << 1);
                gpr[0] = (byte)(tmp & 0xFF);
                gpr[1] = (byte)(tmp >> 8);
                PC++;
            }
            public void MOVW(byte d, byte r)
            {

                gpr[d * 2] = gpr[r * 2];
                gpr[d * 2 + 1] = gpr[r * 2 + 1];
                PC++;
            }
            public void MULS(byte d, byte r)
            {
                ushort tmp = (ushort)(gpr[d] * gpr[r]);
                C = tmp & 0x8000;
                Z = tmp == 0 ? 1 : 0;
                tmp = (ushort)(tmp << 1);
                gpr[0] = (byte)(tmp & 0xFF);
                gpr[1] = (byte)(tmp >> 8);
                PC++;
            }
            /*********************/
            public void CPI(byte d, byte Rr)
            {
                byte Rd = gpr[d + 0x10];


            int res = Rd - Rr;

            H = ~(0x10 + Low(Rd) - Low(Rr)) >> 4;
            V = ((Rd | Rr) ^ res) & 0x80;
            N = res & 0x80;
            Z = res == 0 ? 1 : 0;
            C = res >> 8;
            S = N ^ V;
            PC++;           
            }
            public void SBCI(byte d, byte Rr)
            {
                byte Rd = gpr[d + 0x10];

            int res = Rd - Rr - C;

            H = ~(0x10 + Low(Rd) - Low(Rr) - C) >> 4;
            V = ((Rd | Rr) ^ res) & 0x80;
            N = res & 0x80;
            Z = res == 0 ? 1 : 0;
            C = res >> 8;
            S = N ^ V;
            PC++;
            gpr[d + 0x10] = (byte)(res & 0xFF);
        }
            public void SUBI(byte d, byte Rr)
            {
                byte Rd = gpr[d + 0x10];
                int res = Rd - Rr;

                H = ~(0x10 + Low(Rd) - Low(Rr)) >> 4;
                V = ((Rd | Rr) ^ res) & 0x80;
                N = res & 0x80;
                Z = res == 0 ? 1 : 0;
                C = res >> 8;
                S = N ^ V;
                PC++;
                gpr[d + 0x10] = (byte)(res & 0xFF);
            }
            public void ORI(byte d, byte Rr)
            {
                byte Rd = gpr[d + 0x10];

                int res = Rd | Rr;

                N = res & 0x80;
                Z = res == 0 ? 1 : 0;
                V = 0;
                S = N ^ V;
                gpr[d + 0x10] = (byte)(res & 0xFF);
                PC++;
            }
            public void ANDI(byte d, byte Rr)
            {
                byte Rd = gpr[d + 0x10];

                int res = Rd & Rr;

                N = res & 0x80;
                Z = res == 0 ? 1 : 0;
                V = 0;
                S = N ^ V;
                gpr[d + 0x10] = (byte)(res & 0xFF);
                PC++;
            }
            public void LDI(byte d, byte r)
            {
                gpr[d+0x10] = r;
                PC++;
            }
            public void CBI(byte d, byte r)
            {
                io[d] &= (byte)(255 - (1 << r));
                PC++;
            }
            public void SBI(byte d, byte r)
            {
                io[d] |= (byte)(1 << r);
                PC++;
            }
            public void SBIC(byte d, byte r)
            {
                byte Rd = io[d];
                if ((Rd & (1 << r)) == 0)
                {
                    ushort next = rom[PC + 1];
                    if ((next & 0xFE0E) == 0x940C
                        | (next & 0xFE0E) == 0x940E
                        | (next & 0xFE0F) == 0x9200
                        | (next & 0xFE0F) == 0x9000
                        ) PC += 3;
                    else PC += 2;
                }
                else PC++;
            }
            public void SBIS(byte d, byte r)
            {
                byte Rd = io[d];
                if ((Rd & (1 << r)) > 0)
                {
                    ushort next = rom[PC + 1];
                    if ((next & 0xFE0E) == 0x940C
                        | (next & 0xFE0E) == 0x940E
                        | (next & 0xFE0F) == 0x9200
                        | (next & 0xFE0F) == 0x9000
                        ) PC += 3;
                    else PC += 2;
                }
                else PC++;
            }
            /***/
            public void LDD(byte d, byte r)
            {
                PC++;
            }
            public void STD(byte d, byte r)
            {
                PC++;
            }
            /***/
            public void JMP(int adr)
            {

                PC = adr;
            }
            public void CALL(int adr)
            {
                int SP = ((ushort)io[0x3E] << 8) + io[0x3D];

                SP -= 2;
                PC++;
                ram[SP] = (byte)(PC & 0xFF);
                ram[SP + 1] = (byte)((PC & 0xFF00) >> 8);

                io[0x3E] = (byte)((SP & 0xFF00) >> 8);
                io[0x3D] = (byte)(SP & 0xFF);

                PC = adr;
            }
            public void STS(int adr, byte d)
            {

                ram[adr] = gpr[d];
                PC++;
            }
            public void LDS(int adr, byte d)
            {

                gpr[d] = ram[adr];
                PC++;
            }
        }

    }
