using System;

namespace SalaryCalculator
{
    class Program
    {
        static void Main()
        {
            Console.WriteLine("給与計算PG　計算日数は15日(半月分)　終了:\"end\"");

            // 時給の設定
            int? hourlyWage = ReadIntValue("時給を入力してください");

            // 計算日数の確認
            int days = 15;
            Console.WriteLine("計算日数を加算しますか？ Yes:1 No:0");
            int? additionalDaysOption = ReadIntValue();
            if (additionalDaysOption == 1)
            {
                int? additionalDays = ReadIntValue("加算する日数を入力してください");
                if (additionalDays != null)
                {
                    days += additionalDays.Value;
                }
            }

            // 勤務時間と休憩時間の入力と計算
            int totalWorkHours = 0;
            double totalBreakHours = 0;
            int[] breakTimes = { 0, 0, 0, 0, 18, 24, 30, 48, 54, 60, 72, 84, 96, 108, 114, 120 };
            int[] sal = new int[days];
            for (int i = 0; i < days; i++)
            {
                int? workHours = ReadIntValue($"{i + 1}回目の勤務時間を入力してください (終了: 0)");
                if (workHours == null || workHours == 0)
                {
                    break;
                }
                if (workHours > 15)
                {
                    Console.WriteLine("勤務時間は15" +
                        "以内の数値で入力してください");
                    i--;
                    continue;
                }
                sal[i] = workHours.Value;
                totalWorkHours += sal[i];
                totalBreakHours += breakTimes[sal[i]];
                Console.WriteLine($"勤務時間 {sal[i]} 時間に対応する休憩時間は {breakTimes[sal[i]]} 分です。");
            }

            // 深夜給与の確認
            Console.WriteLine("深夜給与を追加しますか？ Yes:1 No:0");
            int? lateNightOption = ReadIntValue();
            double lateNightSalary = 0;
            if (lateNightOption == 1)
            {
                // 深夜勤務時間の入力
                for (int i = 0; i < days; i++)
                {
                    int? lateNightHours = ReadIntValue($"{i + 1}回目の深夜給与対象勤務時間を入力してください (終了: 0)");
                    if (lateNightHours != null && lateNightHours != 0)
                    {
                        lateNightSalary += lateNightHours.Value * (hourlyWage.Value * 0.25);
                    }
                    else
                    {
                        break;
                    }
                }
            }
            else if (lateNightOption != 0)
            {
                Console.WriteLine("不正な入力です。Yesには1を、Noには0を入力してください。");
                return;
            }

            // 時間外勤務時間の計算
            int overtimeHours = CalculateOvertimeHours(sal);

            // 休憩時間を分から時間に変換
            totalBreakHours /= 60.0;

            // 給与合計の計算
            double totalSalary = CalculateSalary(hourlyWage ?? 0, totalWorkHours, totalBreakHours, lateNightSalary, overtimeHours);

            // 深夜勤務時間の合計
            double totalLateNightHours = lateNightSalary / (hourlyWage.Value * 0.25);

            // 深夜勤務給与合計
            double totalLateNightSalary = lateNightSalary;

            // 時間外勤務給与合計
            double totalOvertimeSalary = overtimeHours * (hourlyWage.Value * 0.25);

            // 休憩時間により引かれた時給分の合計
            double totalBreakSalary = totalBreakHours * hourlyWage.Value;

            // 給与計算内容の表示
            Console.WriteLine($"給与計算内容:");
            Console.WriteLine($"基本時給: {hourlyWage}円");
            Console.WriteLine($"勤務時間: {totalWorkHours} 時間");
            Console.WriteLine($"休憩時間: {totalBreakHours} 時間");
            Console.WriteLine($"休憩時間により引かれた給与合計: {totalBreakSalary} 円");
            Console.WriteLine($"深夜勤務合計: {totalLateNightHours} 時間");
            Console.WriteLine($"深夜勤務給与合計: {totalLateNightSalary} 円");
            Console.WriteLine($"時間外勤務合計: {overtimeHours} 時間");
            Console.WriteLine($"時間外勤務給与合計: {totalOvertimeSalary} 円");

            // 給与合計の表示
            Console.WriteLine($"給与合計: {totalSalary} 円");
        }

        // 時給や計算日数など、数値を入力する際のユーザー入力を処理する関数
        static int? ReadIntValue(string message = "")
        {
            int? value = null;
            while (value == null)
            {
                Console.WriteLine(message);
                string input = Console.ReadLine().Trim().ToLower();
                if (input == "end")
                {
                    Environment.Exit(0);
                }
                if (!string.IsNullOrWhiteSpace(input))
                {
                    if (int.TryParse(input, out int intValue))
                    {
                        value = intValue;
                    }
                    else
                    {
                        Console.WriteLine("入力された値が不正です。数値を入力してください。");
                    }
                }
                else
                {
                    Console.WriteLine("入力された値がNullです。もう一度入力してください。");
                }
            }
            return value;
        }

        // 時間外勤務時間を計算する関数
        static int CalculateOvertimeHours(int[] workHours)
        {
            int overtimeHours = 0;
            foreach (int workHour in workHours)
            {
                if (workHour > 8)
                {
                    overtimeHours += workHour - 8;
                }
            }
            return overtimeHours;
        }

        // 給与を計算する関数
        static double CalculateSalary(int hourlyWage, int totalWorkHours, double totalBreakHours, double lateNightSalary, int overtimeHours)
        {
            double totalSalary = hourlyWage * totalWorkHours - totalBreakHours * hourlyWage + lateNightSalary + overtimeHours * (hourlyWage * 0.25);
            return totalSalary;
        }
    }
}
