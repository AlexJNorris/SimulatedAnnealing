using System;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;

namespace SimulatedAnnealing
{

    class Program
    {
        static string[] RmFrst(string[] str)
        {
            str = str.Where((val, idx) => idx > 0).ToArray();
            return str;
        }

        static int[,] FillRooms()
        {
            int[,] filled = new int[50, 4];
            int room = 0;
            int ind = 0;
            for (int i = 0; i < 200; i++) {
                if (ind == 4)
                {
                    room++;
                    ind = 0;
                }
                filled[room, ind] = i;
                ind++;
            }
            return filled;
        }

        static void Main(string[] args)
        {
            string rawText = File.ReadAllText("roommates.txt");
            string[] rawCompat = rawText.Split(new char[] { ' ' });
            int[,] compat = new int[200, 200];
            int[,] rooms = new int[50, 4];
            rooms = FillRooms();
            int stuNum = 0;
            int ind = 0;


            for (int i = 0; i < rawCompat.Length-1; i++)
            { 
                if ((ind == 200 || i == 0))
                {
                    if (i != 0) { stuNum++; }
                    ind = 0;
                    compat[stuNum, ind] = Int32.Parse(rawCompat[i]);
                    ind++;
                }
                else
                {
                    compat[stuNum, ind] = Int32.Parse(rawCompat[i]);
                    ind++;
                }
            }

        }
    }
}
