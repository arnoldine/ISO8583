using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISO8583_Test
{
   public class f_ISO8583
    {
        int[] DEVarLen = new int[130];
        int[] DEFixLen = new int[130];

        public f_ISO8583()

        {

            //Initialize BitMap Var Length Indicator
            DEVarLen[2] = 2; DEVarLen[32] = 2; DEVarLen[33] = 2; DEVarLen[34] = 2; DEVarLen[35] = 2; DEVarLen[36] = 3;
            DEVarLen[44] = 2; DEVarLen[45] = 2; DEVarLen[46] = 3; DEVarLen[47] = 3; DEVarLen[48] = 3; DEVarLen[54] = 3;
            DEVarLen[55] = 3; DEVarLen[56] = 3; DEVarLen[57] = 3; DEVarLen[58] = 3; DEVarLen[59] = 3;
            DEVarLen[60] = 3; DEVarLen[61] = 3; DEVarLen[62] = 3; DEVarLen[63] = 3; DEVarLen[72] = 3; DEVarLen[99] = 2;
            DEVarLen[100] = 2; DEVarLen[102] = 2; DEVarLen[103] = 2; DEVarLen[104] = 3; DEVarLen[105] = 3;
            DEVarLen[106] = 3; DEVarLen[107] = 3; DEVarLen[108] = 3; DEVarLen[109] = 3; DEVarLen[110] = 3;
            DEVarLen[111] = 3; DEVarLen[112] = 3; DEVarLen[113] = 2; DEVarLen[114] = 3; DEVarLen[115] = 3;
            DEVarLen[116] = 3; DEVarLen[117] = 3; DEVarLen[118] = 3; DEVarLen[119] = 3; DEVarLen[120] = 3; DEVarLen[121] = 3;
            DEVarLen[122] = 3; DEVarLen[123] = 3; DEVarLen[124] = 3; DEVarLen[125] = 2; DEVarLen[126] = 1; DEVarLen[127] = 3;

            // "-" means not numeric.

            DEFixLen[0] = 16; DEFixLen[1] = 16; DEFixLen[3] = 6; DEFixLen[4] = 12;
            DEFixLen[5] = 12; DEFixLen[6] = 12; DEFixLen[7] = 10; DEFixLen[8] = 8;
            DEFixLen[9] = 8; DEFixLen[10] = 8; DEFixLen[11] = 6; DEFixLen[12] = 6;
            DEFixLen[13] = 4; DEFixLen[14] = 4; DEFixLen[15] = 4; DEFixLen[16] = 4;
            DEFixLen[17] = 4; DEFixLen[18] = 4; DEFixLen[19] = 3; DEFixLen[20] = 3;
            DEFixLen[21] = 3; DEFixLen[22] = 3; DEFixLen[23] = 3; DEFixLen[24] = 3;
            DEFixLen[25] = 2; DEFixLen[26] = 2; DEFixLen[27] = 1; DEFixLen[28] = 8;
            DEFixLen[29] = 8; DEFixLen[30] = 8; DEFixLen[31] = 8; DEFixLen[37] = -12;
            DEFixLen[38] = -6; DEFixLen[39] = -2; DEFixLen[40] = -3; DEFixLen[41] = -16;
            DEFixLen[42] = -15; DEFixLen[43] = -40; DEFixLen[49] = -3; DEFixLen[50] = -3;
            DEFixLen[51] = -3; DEFixLen[52] = -16; DEFixLen[53] = 18; DEFixLen[64] = -4;
            DEFixLen[65] = -16; DEFixLen[66] = 1; DEFixLen[67] = 2; DEFixLen[68] = 3;
            DEFixLen[69] = 3; DEFixLen[70] = 3; DEFixLen[71] = 4; DEFixLen[73] = 6;
            DEFixLen[74] = 10; DEFixLen[75] = 10; DEFixLen[76] = 10; DEFixLen[77] = 10;
            DEFixLen[78] = 10; DEFixLen[79] = 10; DEFixLen[80] = 10; DEFixLen[81] = 10;
            DEFixLen[82] = 12; DEFixLen[83] = 12; DEFixLen[84] = 12; DEFixLen[85] = 12;
            DEFixLen[86] = 15; DEFixLen[87] = 15; DEFixLen[88] = 15; DEFixLen[89] = 15;
            DEFixLen[90] = 42; DEFixLen[91] = -1; DEFixLen[92] = 2; DEFixLen[93] = 5;
            DEFixLen[94] = -7; DEFixLen[95] = -42; DEFixLen[96] = -8; DEFixLen[97] = 16;
            DEFixLen[98] = -25; DEFixLen[101] = -17; DEFixLen[128] = -16;

        }
        static string GetField(int number, string m, ref int start)
        {
            int len = 0;
            // Console.WriteLine("Start::{0}", start);
            //if (m.Length - start < 5)
            //{
            //    return string.Empty;
            //}

            switch (number)
            {
                case 2:
                    len = int.Parse(m.Substring(start, 2));
                    start += len + 2;
                    return m.Substring(start - len, len);
                case 3: // n6
                    start += 6;
                    return m.Substring(start - 6, 6);
                case 4: // n16 (STANDARD says n12)
                    start += 12;
                    return m.Substring(start - 12, 12);
                case 7: // n10 or n12
                    start += 10;
                    return m.Substring(start - 10, 10);
                case 8:
                    start += 8;
                    return m.Substring(start - 8, 8);
                case 10:
                    start += 8;
                    return m.Substring(start - 8, 8);
                case 11: // n12 (STANDARD says n6)
                    start += 6;
                    return m.Substring(start - 6, 6);
                case 12: // n14 (STANDARD says n6)
                    start += 6;
                    return m.Substring(start - 6, 6);
                case 13: // n4 or n6
                    start += 4;
                    return m.Substring(start - 4, 4);
                case 17: //n4
                case 14:
                    start += 4;
                    return m.Substring(start - 4, 4);
                case 15: //n4 or n6
                    start += 4;
                    return m.Substring(start - 4, 4);
                //  case 17: // n8 (STANDARD says n4)
                case 18:
                    start += 4;
                    return m.Substring(start - 4, 4);
                case 19:
                    start += 3;
                    return m.Substring(start - 3, 3);

                case 22:
                    start += 12;
                    return m.Substring(start - 12, 12);
                case 28:
                    start += 8;
                    return m.Substring(start - 8, 8);
                case 60:
                    len = int.Parse(m.Substring(start, 3));
                    start += len + 3;
                    return m.Substring(start - len, len);
                case 61:
                    len = int.Parse(m.Substring(start, 3));
                    start += len + 3;
                    return m.Substring(start - len, len);
                case 80:
                case 81:
                case 32:
                   // Console.WriteLine(m.Substring(start, 2));
                    len = int.Parse(m.Substring(start, 2));

                    start += len + 2;
                    return m.Substring(start - len, len);
                //start += 9;
                //return m.Substring(start - 9, 9);
                case 33: // ns..28 (CUSTOM SPEC says ns...28) 
                case 35: // ns..28 (CUSTOM SPEC says ns...28) 
                case 102:
                case 103:
                //case 54:
                    len = int.Parse(m.Substring(start, 2));
                    start += len + 2;
                    return m.Substring(start - len, len);
                case 37:
                    start += 12;
                    return m.Substring(start - 12, 12);
                case 38:
                    start += 6;
                    return m.Substring(start - 6, 6);
                case 39:
                    start += 2;
                    return m.Substring(start - 2, 2);
                case 41: // ans8 (CUSTOM SPEC says ans16)
                    start += 16;
                    return m.Substring(start - 16, 16);
                case 46:
                case 48://n8
                    start += 8;
                    return m.Substring(start - 8, 8);
                case 42: //an15
                    start += 15;
                    return m.Substring(start - 15, 15);
                case 43:
                    start += 40;
                    return m.Substring(start - 40, 40);
                case 49: // n3 
                case 51://n3
                case 52: //n3
                case 70: //n3
                    start += 3;
                    return m.Substring(start - 3, 3);
                case 90: //n44
                    start += 42;
                    return m.Substring(start - 42, 42);
                case 95:
                    start += 56;
                    return m.Substring(start - 56, 56);
                case 123: // ans...999
                case 125: // ans...999
                case 126: // ans...999
                case 127: // ans...999
                case 54:
                    len = int.Parse(m.Substring(start, 3));
                    start += len + 3;
                    return m.Substring(start - len, len);
                default:
                    return "Not yet supported";
            }
        }

        public string[] Parse(string isoMsg)
        {
            //   NLog loinfo= new 
            var loginf = NLog.LogManager.GetCurrentClassLogger();
           
            int[] fields = new int[130];
            string[] DE = new string[130];
            //string isoMsg = "0200F23A401108E00000000000000400000016607142200000114601100000000065000021121313114459524112571821121321121360110000000007KCCBCBS2253D1489400KCCB          MEDICAL COLL.RD.,BASRATGORAKHPUR    UPIN18001300100001008020";
            int mypos = 0;
            int fieldno = 0;
            int mylen = 0;
            //---Field0

            // Initial
            string header = isoMsg.Substring(0, 4);
         //   Console.WriteLine("Message_ID = {0}", header);
            string pb = isoMsg.Substring(4, 16); // primary bitmap
          //  Console.WriteLine(pb);
            var bits = new List<byte>();
            for (int i = 0; i < 16; i++)
            {
                byte b = Convert.ToByte(pb[i].ToString(), 16);
                string s = Convert.ToString(b, 2).PadLeft(4, '0');
                for (int j = 0; j < 4; j++) bits.Add((byte)(s[j] - 48));
            }
            int start = 20;
            if (bits[0] == 1)
            {
                string sb = isoMsg.Substring(20, 16); // secondary bitmap
                start = 36;
             //   Console.WriteLine(sb);
                //if (header == "0800")
                //{    start = 5;sb = m.Substring(4, 16); }
                for (int i = 0; i < 16; i++)
                {
                    byte b = Convert.ToByte(sb[i].ToString(), 16);
                    string s = Convert.ToString(b, 2).PadLeft(4, '0');
                    for (int j = 0; j < 4; j++) bits.Add((byte)(s[j] - 48));
                }
            }
            for (int i = 1; i < bits.Count; i++)
            {

                if (bits[i] == 1)
                {
                    //  Console.WriteLine("{0}", i+1);
                    string f = GetField(i + 1, isoMsg, ref start);
                    DE[i + 1] = f;
                    //Console.WriteLine("Field {0:D3}  = {1}", i + 1, f);
                    string h = string.Format("Field {0:D3}  = {1}", i + 1, f);
                    loginf.Info($"{h}");
                    //if (i >= 124)
                    //{
                    //    if (f.Length == 0) { break; }
                    //    Console.WriteLine("\nMORE TRANSACTION FLAG: {0}", f.Substring(0, 1));
                    //    int not = int.Parse(f.Substring(1, 2));
                    //    Console.WriteLine("NUMBER OF TRANSACTION: {0}", not);
                    //}
                }
            }

            return DE;
        }
        public string Build(string[] DE, string MTI)

        {

            string newISO = MTI;
            string newDE1 = "";

            for (int I = 2; I <= 64; I++) { if (DE[I] != null) { newDE1 += "1"; } else { newDE1 += "0"; } }
            string newDE2 = "";
            for (int I = 65; I <= 128; I++) { if (DE[I] != null) { newDE2 += "1"; } else { newDE2 += "0"; } }
            
            if (newDE2 == "0000000000000000000000000000000000000000000000000000000000000000")
            { newDE1 = "0" + newDE1; }
                        else { newDE1 = "1" + newDE1; }
             string DE1Hex = String.Format("{0:X1}", Convert.ToInt64(newDE1, 2));
            DE1Hex = DE1Hex.PadLeft(16, '0'); //Pad-Left
            DE[0] = DE1Hex;
            string DE2Hex = String.Format("{0:X1}", Convert.ToInt64(newDE2, 2));
            DE2Hex = DE2Hex.PadLeft(16, '0'); //Pad-Left
            DE[1] = DE2Hex;
            if (DE2Hex == "0000000000000000") DE[1] = null;
                                                       
            for (int I = 0; I <= 128; I++)
                {
             //   Console.WriteLine("Field {0}::{1}", I, DE[I]);
                if (DE[I] != null)
                {
                    //if(I==41 || I == 42)
                    //{
                    //    newISO += DE[I].PadRight(16, ' ');
                    //}
                        if (DEVarLen[I] < 1)

                    {
                        if (DEFixLen[I] < 0)

                        {

                            string BMPadded = DE[I].PadRight(Math.Abs(DEFixLen[I]), ' ');
                            string sBM = BMPadded.Substring(0, Math.Abs(DEFixLen[I]));
                            newISO += sBM;

                        }

                        else

                        {

                            string BMPadded = DE[I].PadLeft(DEFixLen[I], '0');

                            string sBM = BMPadded.Substring(BMPadded.Length - Math.Abs(DEFixLen[I]), Math.Abs(DEFixLen[I]));

                            newISO += sBM;

                        }

                    }

                    else

                    {
                        string li = DE[I].Length.ToString();
                        string paddedli = li.PadLeft(DEVarLen[I], '0');
                        newISO += (paddedli + DE[I]);

                    }
                   // Console.WriteLine("Field:: {0}", I);
                }

            }

            return newISO;
        }
        private string DEtoBinary(string HexDE)

        {
            string deBinary = "";
            for (int I = 0; I <= 15; I++)

            {
                deBinary = deBinary + Hex2Binary(HexDE.Substring(I, 1));
            }
            return deBinary;
        }
        private string Hex2Binary(string DE)
        {
            string myBinary = "";
            switch (DE)
            {
                case "0":
                    myBinary = "0000";
                    break;
                case "1":
                    myBinary = "0001";
                    break;
                case "2":
                    myBinary = "0010";
                    break;
                case "3":
                    myBinary = "0011";
                    break;
                case "4":
                    myBinary = "0100";
                    break;
                case "5":
                    myBinary = "0101";
                    break;
                case "6":
                    myBinary = "0110";
                    break;
                case "7":
                    myBinary = "0111";
                    break;
                case "8":
                    myBinary = "1000";
                    break;
                case "9":
                    myBinary = "1001";
                    break;
                case "A":
                    myBinary = "1010";
                    break;
                case "B":
                    myBinary = "1011";
                    break;
                case "C":
                    myBinary = "1100";
                    break;
                case "D":
                    myBinary = "1101";

                    break;
                                   
                case "E":
                    myBinary = "1110";
                    break;
                case "F":
                    myBinary = "1111";

                    break;
            }
            return myBinary;
        }
    }
}
