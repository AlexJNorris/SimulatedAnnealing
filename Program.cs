using System;
using System.IO;
using System.Linq;

namespace SimulatedAnnealing
{
    class Program
    {
        const int STATE_LIMIT = 20000;
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
        static int ComputeAverageFitness(int[,] compat, int[,] rooms)
        {
            int avg = 0;
            for(int i = 0; i < 50; i++)
            {
                avg += ComputeRoomFitness(compat, rooms, i);
            }

            avg /= 50;
            return avg;
        }
        static int ComputeRoomFitness(int[,] compat, int[,] rooms, int roomNum)
        {
            int sum = 0;

            int[] room = new int[4];
            for (int i = 0; i < 4; i++)
            {
                room[i] = rooms[roomNum, i];

                //  Console.WriteLine(room[i]);
            }

            for (int i = 0; i < 4; i++)
            {
                for (int j = i; j < 4; j++)
                {

                    sum += compat[room[i], room[j]];
                }
            }
            return sum;
        }
        static void ConsoleOutRooms(int[,] compat, int[,] rooms)
        {
            int s1, s2, s3, s4, fit, avg;
            avg = ComputeAverageFitness(compat, rooms);
            Console.WriteLine("Average Fitness = " + avg + '\n');
            for(int i = 0; i < 50; i++)
            {
                s1 = rooms[i, 0];
                s2 = rooms[i, 1];
                s3 = rooms[i, 2];
                s4 = rooms[i, 3];
                fit = ComputeRoomFitness(compat, rooms, i);
                Console.WriteLine("Room " + i + " fitness = " + fit + "  { " + s1 + ", " + s2 + ", " + s3 + ", " + s4 + "}");
            }
        }
        //return array of {compatibility, random student in room 1, random student in room 2}
        static int[] TestSwap(int[,] compat, int[,]roomsOG, int swapType, int r1, int r2)
        {
            int[] fit = { 0, 0, 0 };
            int[,] rooms = new int[50,4];
            Random rand = new Random();

            //make copy of roomsOG so the test doesn't perform the swap
            for (int i = 0; i < 50; i++)
            {
                for(int j = 0;j < 4; j++)
                {
                    rooms[i, j] = roomsOG[i, j];
                }
            }

            int temp;

            if (swapType == 0)
            {
                //random student numbers
                int s1 = rand.Next(0, 4);
                int s2 = rand.Next(0, 4);

                temp = rooms[r1, s1];
                rooms[r1, s1] = rooms[r2, s2];
                rooms[r2, s2] = temp;
                fit[1] = s1;
                fit[2] = s2;

            }
            else
            {
                temp = rooms[r1, 0];
                rooms[r1, 0] = rooms[r2, 2];
                rooms[r2, 2] = temp;
                temp = rooms[r1, 1];
                rooms[r1, 1] = rooms[r2, 3];
                rooms[r2, 3] = temp;
            }
            fit[0] += ComputeRoomFitness(compat, rooms, r1);
            fit[0] += ComputeRoomFitness(compat, rooms, r2);
            
            return fit;
        }
        static void Swap(int[,] compat, int[,] rooms, int swapType, int r1, int r2, int s1, int s2)
        {

            int temp;
            if (swapType == 0)
            {
                temp = rooms[r1, s1];
                rooms[r1, s1] = rooms[r2, s2];
                rooms[r2, s2] = temp;
            }
            else
            {
                temp = rooms[r1, 0];
                rooms[r1, 0] = rooms[r2, 2];
                rooms[r2, 2] = temp;
                temp = rooms[r1, 1];
                rooms[r1, 1] = rooms[r2, 3];
                rooms[r2, 3] = temp;
            }
        }
        static int[,] SimulatedAnnealing(int[,] compat, int[,] rooms)
        {
            double t = 10000;
            double alpha = 0.999;
            double epsilon = .001;
            double proba;
            int delta;
            int totalSwaps = 0;
            int totalStates = 0;
            int swaps;
            int states;
            int swapType;
            //room numbers
            int r1, r2;
            //compatibility placeholders
            int c1, c2, cTotal;
            int[] cTest;
            //Random number generator
            Random rand = new Random();


            while (t > epsilon)
            {
                swaps = 0;
                states = 0;

                while (states <= STATE_LIMIT)
                {
                    swapType = rand.Next(0, 2);

                    r1 = rand.Next(0, 50);
                    c1 = ComputeRoomFitness(compat, rooms, r1);
                    r2 = rand.Next(0, 50);
                    c2 = ComputeRoomFitness(compat, rooms, r2);
                    cTotal = c1 + c2;
                    cTest = TestSwap(compat, rooms, swapType, r1, r2);
                    states++;

                    if (cTest[0] < cTotal)
                    {
                        Swap(compat, rooms, swapType, r1, r2, cTest[1], cTest[2]);
                        swaps++;
                        if (swapType == 1)
                        {
                            swaps++;
                        }
                    }
                    else
                    {
                        proba = rand.Next();
                        delta = cTest[0] - cTotal;
                        if(proba < Math.Exp(-delta / t))
                        {
                            Swap(compat, rooms, swapType, r1, r2, cTest[1], cTest[2]);
                            swaps++;
                            if(swapType == 1)
                            {
                                swaps++;
                            }
                        }
                    }
                    if(swaps == 2000)
                    {
                        break;
                    }
                }
                totalStates += states;
                totalSwaps += swaps;
                if (swaps == 0)
                {
                    break;
                }
                t *= alpha;
            }
            Console.WriteLine("Total Swaps = " + totalSwaps + '\n');
            Console.WriteLine("Total States = " + totalStates + '\n');

            return rooms;
        }

        static void Main(string[] args)
        {
            int[,] compat = new int[200, 200];
            string rawText = File.ReadAllText("roommates.txt");
            string[] rawCompat = rawText.Split(new char[] { ' ' });
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

           // ConsoleOutRooms(compat, rooms);
            SimulatedAnnealing(compat, rooms);
            ConsoleOutRooms(compat, rooms);

            //To debug testSwap()
            /*
            int[] sum;
            Random rand = new Random();
            int swapType = rand.Next(0, 2);

            int fit = 0;

            fit += ComputeRoomFitness(compat, rooms, 0);
            fit += ComputeRoomFitness(compat, rooms, 1);
            Console.WriteLine(fit);
            sum = testSwap(compat, rooms, swapType, 0, 1);
            Console.WriteLine(sum[0] + " " + sum[1] + " " + sum[2]);
            fit = 0;

            fit += ComputeRoomFitness(compat, rooms, 0);
            fit += ComputeRoomFitness(compat, rooms, 1);
            Console.WriteLine(fit);
            */


            //To debug Swap()
            /*
            Random rand = new Random();
            int swapType = rand.Next(0, 2);

            int fit = 0;

            fit += ComputeRoomFitness(compat, rooms, 0);
            fit += ComputeRoomFitness(compat, rooms, 1);
            Console.WriteLine(fit);
            Swap(compat, rooms, swapType, 0, 1, 0, 0);
            fit = 0;

            fit += ComputeRoomFitness(compat, rooms, 0);
            fit += ComputeRoomFitness(compat, rooms, 1);
            Console.WriteLine(fit);
            */
        }
    }
}
