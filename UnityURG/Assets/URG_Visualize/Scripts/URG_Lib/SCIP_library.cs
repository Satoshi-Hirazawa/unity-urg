/*!
 * \file
 * \brief SCIP command interface
 * \author Jun Fujimoto
 * $Id: SCIP_library.cs 403 2013-07-11 05:24:12Z fujimoto $
 */

using System;
using System.Collections.Generic;
using System.Text;

namespace SCIP_library
{
    public class SCIP_Writer
    {
        /// <summary>
        /// Create MD command
        /// </summary>
        /// <param name="start">measurement start step</param>
        /// <param name="end">measurement end step</param>
        /// <param name="grouping">grouping step number</param>
        /// <param name="skips">skip scan number</param>
        /// <param name="scans">get scan numbar</param>
        /// <returns>created command</returns>
        public static string MD(int start, int end, int grouping = 1, int skips = 0, int scans = 0)
        {
            return "MD" + start.ToString("D4") + end.ToString("D4") + grouping.ToString("D2") + skips.ToString("D1") + scans.ToString("D2") + "\n";
        }
        public static string MS(int start, int end, int grouping = 1, int skips = 0, int scans = 0) {
            return "MS" + start.ToString("D4") + end.ToString("D4") + grouping.ToString("D2") + skips.ToString("D1") + scans.ToString("D2") + "\n";
        }

        public static string VV()
        {
            return "VV\n";
        }

        public static string II()
        {
            return "II\n";
        }

        public static string PP()
        {
            return "PP\n";
        }

        public static string SCIP2()
        {
            return "SCIP2.0" + "\n";
        }

        public static string QT()
        {
            return "QT\n";
        }
    }

    public class SCIP_Reader
    {
        /// <summary>
        /// read MD command
        /// </summary>
        /// <param name="get_command">received command</param>
        /// <param name="time_stamp">timestamp data</param>
        /// <param name="distances">distance data</param>
        /// <returns>is successful</returns>
        public static bool MD(string get_command, ref long time_stamp, ref List<long> distances)
        {
            
                distances.Clear();
                string[] split_command = get_command.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

                if (!split_command[0].StartsWith("MD")) {
                    return false;
                }

                if (split_command[1].StartsWith("00")) {
                    return true;
                } else if (split_command[1].StartsWith("99")) {
                    time_stamp = SCIP_Reader.decode_long(split_command[2], 4);
                    distance_data(split_command, 3, ref distances);
                    return true;
                } else {
                    return false;
                }
            
        }

        public static bool MS(string get_command, ref long time_stamp, ref List<int> distances) {

            distances.Clear();
            string[] split_command = get_command.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

            if (!split_command[0].StartsWith("MS")) {
                return false;
            }

            if (split_command[1].StartsWith("00")) {
                return true;
            } else if (split_command[1].StartsWith("99")) {
                time_stamp = SCIP_Reader.decode_long(split_command[2], 4);
                distance_data(split_command, 3, ref distances, 2);
                return true;
            } else {
                return false;
            }

        }

        /// <summary>
        /// read distance data
        /// </summary>
        /// <param name="lines"></param>
        /// <param name="start_line"></param>
        /// <returns></returns>
        public static bool distance_data(string[] lines, int start_line, ref List<long> distances, int size = 3)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = start_line; i < lines.Length; ++i) {
                sb.Append(lines[i].Substring(0, lines[i].Length - 1));
            }
            return SCIP_Reader.decode_array(sb.ToString(), size, ref distances);
        }

        public static bool distance_data(string[] lines, int start_line, ref List<int> distances, int size = 2) {
            StringBuilder sb = new StringBuilder();
            for (int i = start_line; i < lines.Length; ++i) {
                sb.Append(lines[i].Substring(0, lines[i].Length - 1));
            }
            return SCIP_Reader.decode_array(sb.ToString(), size, ref distances);
        }

        /// <summary>
        /// decode part of string 
        /// </summary>
        /// <param name="data">encoded string</param>
        /// <param name="size">encode size</param>
        /// <param name="offset">decode start position</param>
        /// <returns>decode result</returns>
        public static long decode_long(string data, int size, int offset = 0)
        {
            long value = 0;

            for (int i = 0; i < size; ++i) {
                value <<= 6;
                value |= (long)data[offset + i] - 0x30;
            }

            return value;
        }

        public static int decode_int(string data, int size, int offset = 0) {
            int value = 0;

            for (int i = 0; i < size; ++i) {
                value <<= 6;
                value |= (int)data[offset + i] - 0x30;
            }

            return value;
        }

        /// <summary>
        /// decode multiple data
        /// </summary>
        /// <param name="data">encoded string</param>
        /// <param name="size">encode size</param>
        /// <returns>decode result</returns>
        public static bool decode_array(string data, int size, ref List<long> decoded_data)
        {
            for (int pos = 0; pos <= data.Length - size; pos += size) {
                decoded_data.Add(decode_long(data, size, pos));
            }
            return true;
        }

        public static bool decode_array(string data, int size, ref List<int> decoded_data) {
            for (int pos = 0; pos <= data.Length - size; pos += size) {
                decoded_data.Add(decode_int(data, size, pos));
            }
            return true;
        }

        public static bool PP(string data, ref SCIP_Parameter decoded_data) {
            string[] split_command = data.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

            if (!split_command[0].StartsWith("PP")) {
                return false;
            }
            if (split_command.Length < 10) {
                return false;
            }
            if (split_command[1].StartsWith("00")) {

                decoded_data.MODL = split_command[2].Substring(5, split_command[2].Length - 7);
                int.TryParse(split_command[3].Substring(5, split_command[3].Length - 7), out decoded_data.DMIN);
                int.TryParse(split_command[4].Substring(5, split_command[4].Length - 7), out decoded_data.DMAX);
                int.TryParse(split_command[5].Substring(5, split_command[5].Length - 7), out decoded_data.ARES);
                int.TryParse(split_command[6].Substring(5, split_command[6].Length - 7), out decoded_data.AMIN);
                int.TryParse(split_command[7].Substring(5, split_command[7].Length - 7), out decoded_data.AMAX);
                int.TryParse(split_command[8].Substring(5, split_command[8].Length - 7), out decoded_data.AFRT);
                int.TryParse(split_command[9].Substring(5, split_command[9].Length - 7), out decoded_data.SCAN);
                return true;
            } else {
                return false;
            }
        }
    }

    public struct SCIP_Parameter {
        public string MODL;
        public int DMIN;
        public int DMAX;
        public int ARES;
        public int AMIN;
        public int AMAX;
        public int AFRT;
        public int SCAN;
    }
}
