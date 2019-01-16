using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Possible.Math.Enum;

namespace Possible.Math
{
	public partial class Financial
	{
		
		/// <summary>Returns a <see langword="decimal" /> specifying the interest payment for a given period of an annuity based on periodic, fixed payments and a fixed interest rate.</summary>
		/// <param name="Rate">Required. <see langword="decimal" /> specifying interest rate per period. For example, if you get a car loan at an annual percentage rate (APR) of 10 percent and make monthly payments, the rate per period is 0.1/12, or 0.0083.</param>
		/// <param name="Per">Required. <see langword="decimal" /> specifying payment period in the range 1 through <paramref name="NPer" />.</param>
		/// <param name="NPer">Required. <see langword="decimal" /> specifying total number of payment periods in the annuity. For example, if you make monthly payments on a four-year car loan, your loan has a total of 4 x 12 (or 48) payment periods.</param>
		/// <param name="PV">Required. <see langword="decimal" /> specifying present value, or value today, of a series of future payments or receipts. For example, when you borrow money to buy a car, the loan amount is the present value to the lender of the monthly car payments you will make.</param>
		/// <param name="FV">Optional. <see langword="decimal" /> specifying future value or cash balance you want after you've made the final payment. For example, the future value of a loan is $0 because that's its value after the final payment. However, if you want to save $50,000 over 18 years for your child's education, then $50,000 is the future value. If omitted, 0 is assumed.</param>
		/// <param name="Due">Optional. Object of type <see cref="T:Microsoft.VisualBasic.DueDate" /> that specifies when payments are due. This argument must be either DueDate.EndOfPeriod if payments are due at the end of the payment period, or DueDate.BegOfPeriod if payments are due at the beginning of the period. If omitted, DueDate.EndOfPeriod is assumed.</param>
		/// <returns>Returns a <see langword="decimal" /> specifying the interest payment for a given period of an annuity based on periodic, fixed payments and a fixed interest rate.</returns>
		/// <exception cref="T:System.ArgumentException">
		/// <paramref name="Per" /> &lt;= 0 or <paramref name="Per" /> &gt; <paramref name="NPer" /></exception>
		public static decimal IPmt(
		  decimal Rate,
		  decimal Per,
		  decimal NPer,
		  decimal PV,
		  decimal FV = 0.0M,
		  DueDate Due = DueDate.EndOfPeriod)
		{
			var num1 = Due == DueDate.EndOfPeriod ? 1.0M : 2.0M;
			if (Per <= 0.0M || Per >= NPer + 1.0M)
				throw new ArgumentException("Argument_InvalidValue Per");
			decimal num2;
			if (Due != DueDate.EndOfPeriod && Per == 1.0M)
			{
				num2 = 0.0M;
			}
			else
			{
				var Pmt = PMT_Internal(Rate, NPer, PV, FV, Due);
				if (Due != DueDate.EndOfPeriod)
					PV += Pmt;
				num2 = FV_Internal(Rate, Per - num1, Pmt, PV, DueDate.EndOfPeriod) * Rate;
			}
			return num2;
		}

		/// <summary>Returns a <see langword="decimal" /> specifying the internal rate of return for a series of periodic cash flows (payments and receipts).</summary>
		/// <param name="ValueArray">Required. Array of <see langword="decimal" /> specifying cash flow values. The array must contain at least one negative value (a payment) and one positive value (a receipt).</param>
		/// <param name="Guess">Optional. Object specifying value you estimate will be returned by <see langword="IRR" />. If omitted, <paramref name="Guess" /> is 0.1 (10 percent).</param>
		/// <returns>Returns a <see langword="decimal" /> specifying the internal rate of return for a series of periodic cash flows (payments and receipts).</returns>
		/// <exception cref="T:System.ArgumentException">Array argument values are invalid or <paramref name="Guess" /> &lt;= -1.</exception>
		public static decimal IRR(
			ref decimal[] ValueArray,
			decimal Guess = 0.1M)
		{
			int upperBound;
			try
			{
				upperBound = ValueArray.GetUpperBound(0);
			}
			catch (StackOverflowException ex)
			{
				throw ex;
			}
			catch (OutOfMemoryException ex)
			{
				throw ex;
			}
			catch (ThreadAbortException ex)
			{
				throw ex;
			}
			catch (Exception ex)
			{
				throw new ArgumentException("Argument_InvalidValue ValueArray");
			}
			var num1 = checked(upperBound + 1);
			if (Guess <= -1.0M)
				throw new ArgumentException("Argument_InvalidValue Guess");
			if (num1 <= 1)
				throw new ArgumentException("Argument_InvalidValue ValueArray");
			var num2 = ValueArray[0] <= 0.0M ? -ValueArray[0] : ValueArray[0];
			var num3 = upperBound;
			var index = 0;
			while (index <= num3)
			{
				if (ValueArray[index] > num2)
					num2 = ValueArray[index];
				else if (-ValueArray[index] > num2)
					num2 = -ValueArray[index];
				checked { ++index; }
			}
			var num4 = num2 * 1E-07M * 0.01M;
			var Guess1 = Guess;
			var num5 = OptPV2(ref ValueArray, Guess1);
			var Guess2 = num5 <= 0.0M ? Guess1 - 1E-05M : Guess1 + 1E-05M;
			if (Guess2 <= -1.0M)
				throw new ArgumentException("Argument_InvalidValue Rate");
			var num6 = OptPV2(ref ValueArray, Guess2);
			var num7 = 0;
			do
			{
				if (num6 == num5)
				{
					if (Guess2 > Guess1)
						Guess1 -= 1E-05M;
					else
						Guess1 += 1E-05M;
					num5 = OptPV2(ref ValueArray, Guess1);
					if (num6 == num5)
						throw new ArgumentException("Argument_InvalidValue");
				}
				var num8 = Guess2;
				var Guess3 = num8 - (num8 - Guess1) * num6 / (num6 - num5);
				if (Guess3 <= -1.0M)
					Guess3 = (Guess2 - 1.0M) * 0.5M;
				var num9 = OptPV2(ref ValueArray, Guess3);
				var num10 = Guess3 <= Guess2 ? Guess2 - Guess3 : Guess3 - Guess2;
				if ((num9 <= 0.0M ? -num9 : num9) < num4 && num10 < 1E-07M)
					return Guess3;
				var num11 = num9;
				num5 = num6;
				num6 = num11;
				var num12 = Guess3;
				Guess1 = Guess2;
				Guess2 = num12;
				checked { ++num7; }
			}
			while (num7 <= 39);
			throw new ArgumentException("Argument_InvalidValue");
		}

		/// <summary>Returns a <see langword="decimal" /> specifying the payment for an annuity based on periodic, fixed payments and a fixed interest rate.</summary>
		/// <param name="Rate">Required. <see langword="decimal" /> specifies the interest rate per period. For example, if you get a car loan at an annual percentage rate (APR) of 10 percent and make monthly payments, the rate per period is 0.1/12, or 0.0083.</param>
		/// <param name="NPer">Required. <see langword="decimal" /> specifies the total number of payment periods in the annuity. For example, if you make monthly payments on a four-year car loan, your loan has a total of 4 × 12 (or 48) payment periods.</param>
		/// <param name="PV">Required. <see langword="decimal" /> specifies the present value (or lump sum) that a series of payments to be paid in the future is worth now. For example, when you borrow money to buy a car, the loan amount is the present value to the lender of the monthly car payments you will make.</param>
		/// <param name="FV">Optional. <see langword="decimal" /> specifying future value or cash balance you want after you have made the final payment. For example, the future value of a loan is $0 because that is its value after the final payment. However, if you want to save $50,000 during 18 years for your child's education, then $50,000 is the future value. If omitted, 0 is assumed.</param>
		/// <param name="Due">Optional. Object of type <see cref="T:Microsoft.VisualBasic.DueDate" /> that specifies when payments are due. This argument must be either DueDate.EndOfPeriod if payments are due at the end of the payment period, or DueDate.BegOfPeriod if payments are due at the beginning of the period. If omitted, DueDate.EndOfPeriod is assumed.</param>
		/// <returns>Returns a <see langword="decimal" /> specifying the payment for an annuity based on periodic, fixed payments and a fixed interest rate.</returns>
		/// <exception cref="T:System.ArgumentException">
		/// <paramref name="NPer" /> = 0.</exception>
		public static decimal Pmt(
			decimal Rate,
			decimal NPer,
			decimal PV,
			decimal FV = 0.0M,
			DueDate Due = DueDate.EndOfPeriod)
		{
			return PMT_Internal(Rate, NPer, PV, FV, Due);
		}

		private static decimal PMT_Internal(
		  decimal Rate,
		  decimal NPer,
		  decimal PV,
		  decimal FV = 0.0M,
		  DueDate Due = DueDate.EndOfPeriod)
		{
			if (NPer == 0.0M)
				throw new ArgumentException("Argument_InvalidValue NPer");
			decimal num1;
			if (Rate == 0.0M)
			{
				num1 = (-FV - PV) / NPer;
			}
			else
			{
				var num2 = Due == DueDate.EndOfPeriod ? 1.0M : 1.0M + Rate;
				var num3 = (decimal)System.Math.Pow((double)Rate + 1.0, (double)NPer);
				num1 = (-FV - PV * num3) / (num2 * (num3 - 1.0M)) * Rate;
			}
			return num1;
		}

		/// <summary>Returns a <see langword="decimal" /> specifying the principal payment for a given period of an annuity based on periodic fixed payments and a fixed interest rate.</summary>
		/// <param name="Rate">Required. <see langword="decimal" /> specifies the interest rate per period. For example, if you get a car loan at an annual percentage rate (APR) of 10 percent and make monthly payments, the rate per period is 0.1/12, or 0.0083.</param>
		/// <param name="Per">Required. <see langword="decimal" /> specifies the payment period in the range 1 through <paramref name="NPer" />.</param>
		/// <param name="NPer">Required. <see langword="decimal" /> specifies the total number of payment periods in the annuity. For example, if you make monthly payments on a four-year car loan, your loan has a total of 4 x 12 (or 48) payment periods.</param>
		/// <param name="PV">Required. <see langword="decimal" /> specifies the current value of a series of future payments or receipts. For example, when you borrow money to buy a car, the loan amount is the present value to the lender of the monthly car payments you will make.</param>
		/// <param name="FV">Optional. <see langword="decimal" /> specifying future value or cash balance you want after you have made the final payment. For example, the future value of a loan is $0 because that is its value after the final payment. However, if you want to save $50,000 over 18 years for your child's education, then $50,000 is the future value. If omitted, 0 is assumed.</param>
		/// <param name="Due">Optional. Object of type <see cref="T:Microsoft.VisualBasic.DueDate" /> that specifies when payments are due. This argument must be either DueDate.EndOfPeriod if payments are due at the end of the payment period, or DueDate.BegOfPeriod if payments are due at the beginning of the period. If omitted, DueDate.EndOfPeriod is assumed.</param>
		/// <returns>Returns a <see langword="decimal" /> specifying the principal payment for a given period of an annuity based on periodic fixed payments and a fixed interest rate.</returns>
		/// <exception cref="T:System.ArgumentException">
		/// <paramref name="Per" /> &lt;=0 or <paramref name="Per" /> &gt; <paramref name="NPer" />.</exception>
		public static decimal PPmt(
		  decimal Rate,
		  decimal Per,
		  decimal NPer,
		  decimal PV,
		  decimal FV = 0.0M,
		  DueDate Due = DueDate.EndOfPeriod)
		{
			if (Per <= 0.0M || Per >= NPer + 1.0M)
				throw new ArgumentException("PPMT_PerGT0AndLTNPer");
			return PMT_Internal(Rate, NPer, PV, FV, Due) - IPmt(Rate, Per, NPer, PV, FV, Due);
		}


		private static decimal OptPV2(
			ref decimal[] ValueArray,
			decimal Guess = 0.1M)
		{
			var index1 = 0;
			var upperBound = ValueArray.GetUpperBound(0);
			var num1 = 0.0M;
			var num2 = 1.0M + Guess;
			while (index1 <= upperBound && ValueArray[index1] == 0.0M)
				checked { ++index1; }
			var num3 = upperBound;
			var num4 = index1;
			var index2 = num3;
			while (index2 >= num4)
			{
				num1 = num1 / num2 + ValueArray[index2];
				checked { index2 += -1; }
			}
			return num1;
		}

		private static decimal FV_Internal(
			decimal Rate,
			decimal NPer,
			decimal Pmt,
			decimal PV = 0.0M,
			DueDate Due = DueDate.EndOfPeriod)
		{
			decimal num1;
			if (Rate == 0.0M)
			{
				num1 = -PV - Pmt * NPer;
			}
			else
			{
				var num2 = Due == DueDate.EndOfPeriod ? 1.0M : 1.0M + Rate;
				var num3 = (decimal)System.Math.Pow(1.0 + (double)Rate, (double)NPer);
				num1 = -PV * num3 - Pmt / Rate * num2 * (num3 - 1.0M);
			}
			return num1;
		}

		private static decimal LDoNPV(
			decimal Rate,
			ref decimal[] ValueArray,
			int iWNType)
		{
			var flag1 = iWNType < 0;
			var flag2 = iWNType > 0;
			var num1 = 1.0M;
			var num2 = 0.0M;
			var upperBound = ValueArray.GetUpperBound(0);
			var index = 0;
			while (index <= upperBound)
			{
				var num3 = ValueArray[index];
				var num4 = num1;
				num1 = num4 + num4 * Rate;
				if ((!flag1 || num3 <= 0.0M) && (!flag2 || num3 >= 0.0M))
					num2 += num3 / num1;
				checked { ++index; }
			}
			return num2;
		}

		private static decimal LEvalRate(
			decimal Rate,
			decimal NPer,
			decimal Pmt,
			decimal PV,
			decimal dFv,
			DueDate Due)
		{
			decimal num1;
			if (Rate == 0.0M)
			{
				num1 = PV + Pmt * NPer + dFv;
			}
			else
			{
				var num2 = (decimal)System.Math.Pow((double)(Rate + 1.0M), (double)NPer);
				var num3 = Due == DueDate.EndOfPeriod ? 1.0M : 1.0M + Rate;
				num1 = PV * num2 + Pmt * num3 * (num2 - 1.0M) / Rate + dFv;
			}
			return num1;
		}

		/// <summary>Returns a <see langword="decimal" /> specifying the sum-of-years digits depreciation of an asset for a specified period.</summary>
		/// <param name="Cost">Required. <see langword="decimal" /> specifying the initial cost of the asset.</param>
		/// <param name="Salvage">Required. <see langword="decimal" /> specifying the value of the asset at the end of its useful life.</param>
		/// <param name="Life">Required. <see langword="decimal" /> specifying the length of the useful life of the asset.</param>
		/// <param name="Period">Required. <see langword="decimal" /> specifying the period for which asset depreciation is calculated.</param>
		/// <returns>Returns a <see langword="decimal" /> specifying the sum-of-years digits depreciation of an asset for a specified period.</returns>
		/// <exception cref="T:System.ArgumentException">
		/// <paramref name="Salvage" /> &lt; 0, <paramref name="Period" /> &gt; <paramref name="Life" />, or <paramref name="Period" /> &lt;=0.</exception>
		public static decimal SYD(
			decimal Cost,
			decimal Salvage,
			decimal Life,
			decimal Period)
		{
			if (Salvage < 0.0M)
				throw new ArgumentException("Financial_ArgGEZero Salvage");
			if (Period > Life)
				throw new ArgumentException("Financial_PeriodLELife");
			if (Period <= 0.0M)
				throw new ArgumentException("Financial_ArgGTZero Period");
			var num1 = Cost - Salvage;
			var num2 = Life;
			var num3 = num2 * (num2 + 1.0M);
			return num1 / num3 * (Life + 1.0M - Period) * 2.0M;
		}

		/// <summary>Returns a <see langword="decimal" /> specifying the straight-line depreciation of an asset for a single period.</summary>
		/// <param name="Cost">Required. <see langword="decimal" /> specifying initial cost of the asset.</param>
		/// <param name="Salvage">Required. <see langword="decimal" /> specifying value of the asset at the end of its useful life.</param>
		/// <param name="Life">Required. <see langword="decimal" /> specifying length of the useful life of the asset.</param>
		/// <returns>Returns a <see langword="decimal" /> specifying the straight-line depreciation of an asset for a single period.</returns>
		/// <exception cref="T:System.ArgumentException">
		/// <paramref name="Life" /> = 0.</exception>
		public static decimal SLN(
			decimal Cost,
			decimal Salvage,
			decimal Life)
		{
			if (Life == 0.0M)
				throw new ArgumentException("Financial_LifeNEZero");
			return (Cost - Salvage) / Life;
		}

		/// <summary>Returns a <see langword="decimal" /> specifying the interest rate per period for an annuity.</summary>
		/// <param name="NPer">Required. <see langword="decimal" /> specifies the total number of payment periods in the annuity. For example, if you make monthly payments on a four-year car loan, your loan has a total of 4 * 12 (or 48) payment periods.</param>
		/// <param name="Pmt">Required. <see langword="decimal" /> specifies the payment to be made each period. Payments usually contain principal and interest that doesn't change over the life of the annuity.</param>
		/// <param name="PV">Required. <see langword="decimal" /> specifies the present value, or value today, of a series of future payments or receipts. For example, when you borrow money to buy a car, the loan amount is the present value to the lender of the monthly car payments you will make.</param>
		/// <param name="FV">Optional. <see langword="decimal" /> specifies the future value or cash balance you want after you make the final payment. For example, the future value of a loan is $0 because that is its value after the final payment. However, if you want to save $50,000 over 18 years for your child's education, then $50,000 is the future value. If omitted, 0 is assumed.</param>
		/// <param name="Due">Optional. Object of type <see cref="T:Microsoft.VisualBasic.DueDate" /> that specifies when payments are due. This argument must be either DueDate.EndOfPeriod if payments are due at the end of the payment period, or DueDate.BegOfPeriod if payments are due at the beginning of the period. If omitted, DueDate.EndOfPeriod is assumed.</param>
		/// <param name="Guess">Optional. <see langword="decimal" /> specifying value you estimate is returned by <see langword="Rate" />. If omitted, <paramref name="Guess" /> is 0.1 (10 percent).</param>
		/// <returns>Returns a <see langword="decimal" /> specifying the interest rate per period for an annuity.</returns>
		/// <exception cref="T:System.ArgumentException">
		/// <paramref name="NPer" /> &lt;= 0.</exception>
		public static decimal Rate(
		  decimal NPer,
		  decimal Pmt,
		  decimal PV,
		  decimal FV = 0.0M,
		  DueDate Due = DueDate.EndOfPeriod,
		  decimal Guess = 0.1M)
		{
			if (NPer <= 0.0M)
				throw new ArgumentException("Rate_NPerMustBeGTZero");
			var Rate1 = Guess;
			var num1 = LEvalRate(Rate1, NPer, Pmt, PV, FV, Due);
			var Rate2 = num1 <= 0.0M ? Rate1 * 2.0M : Rate1 / 2.0M;
			var num2 = LEvalRate(Rate2, NPer, Pmt, PV, FV, Due);
			var num3 = 0;
			do
			{
				if (num2 == num1)
				{
					if (Rate2 > Rate1)
						Rate1 -= 1E-05M;
					else
						Rate1 -= -1E-05M;
					num1 = LEvalRate(Rate1, NPer, Pmt, PV, FV, Due);
					if (num2 == num1)
						throw new ArgumentException("Financial_CalcDivByZero");
				}
				var num4 = Rate2;
				var Rate3 = num4 - (num4 - Rate1) * num2 / (num2 - num1);
				var num5 = LEvalRate(Rate3, NPer, Pmt, PV, FV, Due);
				if (System.Math.Abs(num5) < 1E-07M)
					return Rate3;
				var num6 = num5;
				num1 = num2;
				num2 = num6;
				var num7 = Rate3;
				Rate1 = Rate2;
				Rate2 = num7;
				checked { ++num3; }
			}
			while (num3 <= 39);
			throw new ArgumentException("Financial_CannotCalculateRate");
		}

		/// <summary>Returns a <see langword="decimal" /> specifying the present value of an annuity based on periodic, fixed payments to be paid in the future and a fixed interest rate.</summary>
		/// <param name="Rate">Required. <see langword="decimal" /> specifies the interest rate per period. For example, if you get a car loan at an annual percentage rate (APR) of 10 percent and make monthly payments, the rate per period is 0.1/12, or 0.0083.</param>
		/// <param name="NPer">Required. <see langword="decimal" /> specifies the total number of payment periods in the annuity. For example, if you make monthly payments on a four-year car loan, your loan has 4 x 12 (or 48) payment periods.</param>
		/// <param name="Pmt">Required. <see langword="decimal" /> specifies the payment to be made each period. Payments usually contain principal and interest that does not change during the life of the annuity.</param>
		/// <param name="FV">Optional. <see langword="decimal" /> specifies the future value or cash balance you want after you make the final payment. For example, the future value of a loan is $0 because that is its value after the final payment. However, if you want to save $50,000 over 18 years for your child's education, then $50,000 is the future value. If omitted, 0 is assumed.</param>
		/// <param name="Due">Optional. Object of type <see cref="T:Microsoft.VisualBasic.DueDate" /> that specifies when payments are due. This argument must be either DueDate.EndOfPeriod if payments are due at the end of the payment period, or DueDate.BegOfPeriod if payments are due at the beginning of the period. If omitted, DueDate.EndOfPeriod is assumed.</param>
		/// <returns>Returns a <see langword="decimal" /> specifying the present value of an annuity based on periodic, fixed payments to be paid in the future and a fixed interest rate.</returns>
		public static decimal PV(
			decimal Rate,
			decimal NPer,
			decimal Pmt,
			decimal FV = 0.0M,
			DueDate Due = DueDate.EndOfPeriod)
		{
			decimal num1;
			if (Rate == 0.0M)
			{
				num1 = -FV - Pmt * NPer;
			}
			else
			{
				var num2 = Due == DueDate.EndOfPeriod ? 1.0M : 1.0M + Rate;
				var num3 = (decimal)System.Math.Pow(1.0 + (double)Rate, (double)NPer);
				num1 = -(FV + Pmt * num2 * ((num3 - 1.0M) / Rate)) / num3;
			}
			return num1;
		}

		/// <summary>Returns a <see langword="decimal" /> specifying the net present value of an investment based on a series of periodic cash flows (payments and receipts) and a discount rate.</summary>
		/// <param name="Rate">Required. <see langword="decimal" /> specifying discount rate over the length of the period, expressed as a decimal.</param>
		/// <param name="ValueArray">Required. Array of <see langword="decimal" /> specifying cash flow values. The array must contain at least one negative value (a payment) and one positive value (a receipt).</param>
		/// <returns>Returns a <see langword="decimal" /> specifying the net present value of an investment based on a series of periodic cash flows (payments and receipts) and a discount rate.</returns>
		/// <exception cref="T:System.ArgumentException">
		/// <paramref name="ValueArray" /> is <see langword="Nothing" />, rank of <paramref name="ValueArray" /> &lt;&gt; 1, or <paramref name="Rate" /> = -1 </exception>
		public static decimal NPV(
			decimal Rate,
			ref decimal[] ValueArray)
		{
			if (ValueArray == null)
				throw new ArgumentException("Argument_InvalidNullValue ValueArray");
			if (ValueArray.Rank != 1)
				throw new ArgumentException("Argument_RankEQOne ValueArray");
			var num1 = 0;
			var num2 = checked(ValueArray.GetUpperBound(0) - num1 + 1);
			if (Rate == -1.0M)
				throw new ArgumentException("Argument_InvalidValue Rate");
			if (num2 < 1)
				throw new ArgumentException("Argument_InvalidValue ValueArray");
			return LDoNPV(Rate, ref ValueArray, 0);
		}

		/// <summary>Returns a <see langword="decimal" /> specifying the number of periods for an annuity based on periodic fixed payments and a fixed interest rate.</summary>
		/// <param name="Rate">Required. <see langword="decimal" /> specifying interest rate per period. For example, if you get a car loan at an annual percentage rate (APR) of 10 percent and make monthly payments, the rate per period is 0.1/12, or 0.0083.</param>
		/// <param name="Pmt">Required. <see langword="decimal" /> specifying payment to be made each period. Payments usually contain principal and interest that does not change over the life of the annuity.</param>
		/// <param name="PV">Required. <see langword="decimal" /> specifying present value, or value today, of a series of future payments or receipts. For example, when you borrow money to buy a car, the loan amount is the present value to the lender of the monthly car payments you will make.</param>
		/// <param name="FV">Optional. <see langword="decimal" /> specifying future value or cash balance you want after you have made the final payment. For example, the future value of a loan is $0 because that is its value after the final payment. However, if you want to save $50,000 over 18 years for your child's education, then $50,000 is the future value. If omitted, 0 is assumed.</param>
		/// <param name="Due">Optional. Object of type <see cref="T:Microsoft.VisualBasic.DueDate" /> that specifies when payments are due. This argument must be either DueDate.EndOfPeriod if payments are due at the end of the payment period, or DueDate.BegOfPeriod if payments are due at the beginning of the period. If omitted, DueDate.EndOfPeriod is assumed.</param>
		/// <returns>Returns a <see langword="decimal" /> specifying the number of periods for an annuity based on periodic fixed payments and a fixed interest rate.</returns>
		/// <exception cref="T:System.ArgumentException">
		/// <paramref name="Rate" /> &lt;= -1.</exception>
		/// <exception cref="T:System.ArgumentException">
		/// <paramref name="Rate" /> = 0 and <paramref name="Pmt" /> = 0</exception>
		public static decimal NPer(
			decimal Rate,
			decimal Pmt,
			decimal PV,
			decimal FV = 0.0M,
			DueDate Due = DueDate.EndOfPeriod)
		{
			if (Rate <= -1.0M)
				throw new ArgumentException("Argument_InvalidValue Rate");
			decimal num1;
			if (Rate == 0.0M)
			{
				if (Pmt == 0.0M)
					throw new ArgumentException("Argument_InvalidValue Pmt");
				num1 = -(PV + FV) / Pmt;
			}
			else
			{
				var num2 = Due == DueDate.EndOfPeriod ? Pmt / Rate : Pmt * (1.0M + Rate) / Rate;
				var d1 = -FV + num2;
				var d2 = PV + num2;
				if (d1 < 0.0M && d2 < 0.0M)
				{
					d1 = -1.0M * d1;
					d2 = -1.0M * d2;
				}
				else if (d1 <= 0.0M || d2 <= 0.0M)
					throw new ArgumentException("Financial_CannotCalculateNPer");
				var d3 = Rate + 1.0M;
				num1 = (decimal)((System.Math.Log((double)d1) - System.Math.Log((double)d2)) / System.Math.Log((double)d3));
			}
			return num1;
		}

		/// <summary>Returns a <see langword="decimal" /> specifying the future value of an annuity based on periodic, fixed payments and a fixed interest rate.</summary>
		/// <param name="Rate">Required. <see langword="decimal" /> specifying interest rate per period. For example, if you get a car loan at an annual percentage rate (APR) of 10 percent and make monthly payments, the rate per period is 0.1/12, or 0.0083.</param>
		/// <param name="NPer">Required. <see langword="decimal" /> specifying total number of payment periods in the annuity. For example, if you make monthly payments on a four-year car loan, your loan has a total of 4 x 12 (or 48) payment periods.</param>
		/// <param name="Pmt">Required. <see langword="decimal" /> specifying payment to be made each period. Payments usually contain principal and interest that doesn't change over the life of the annuity.</param>
		/// <param name="PV">Optional. <see langword="decimal" /> specifying present value (or lump sum) of a series of future payments. For example, when you borrow money to buy a car, the loan amount is the present value to the lender of the monthly car payments you will make. If omitted, 0 is assumed.</param>
		/// <param name="Due">Optional. Object of type <see cref="T:Microsoft.VisualBasic.DueDate" /> that specifies when payments are due. This argument must be either <see langword="DueDate.EndOfPeriod" /> if payments are due at the end of the payment period, or <see langword="DueDate.BegOfPeriod" /> if payments are due at the beginning of the period. If omitted, <see langword="DueDate.EndOfPeriod" /> is assumed.</param>
		/// <returns>Returns a <see langword="decimal" /> specifying the future value of an annuity based on periodic, fixed payments and a fixed interest rate.</returns>
		public static decimal FV(
			decimal Rate,
			decimal NPer,
			decimal Pmt,
			decimal PV = 0.0M,
			DueDate Due = DueDate.EndOfPeriod)
		{
			return FV_Internal(Rate, NPer, Pmt, PV, Due);
		}

		/// <summary>Returns a <see langword="decimal" /> specifying the depreciation of an asset for a specific time period using the decimal-declining balance method or some other method you specify.</summary>
		/// <param name="Cost">Required. <see langword="decimal" /> specifying initial cost of the asset.</param>
		/// <param name="Salvage">Required. <see langword="decimal" /> specifying value of the asset at the end of its useful life.</param>
		/// <param name="Life">Required. <see langword="decimal" /> specifying length of useful life of the asset.</param>
		/// <param name="Period">Required. <see langword="decimal" /> specifying period for which asset depreciation is calculated.</param>
		/// <param name="Factor">Optional. <see langword="decimal" /> specifying rate at which the balance declines. If omitted, 2 (decimal-declining method) is assumed.</param>
		/// <returns>Returns a <see langword="decimal" /> specifying the depreciation of an asset for a specific time period using the decimal-declining balance method or some other method you specify.</returns>
		/// <exception cref="T:System.ArgumentException">
		/// <paramref name="Factor" /> &lt;= 0, <paramref name="Salvage" /> &lt; 0, <paramref name="Period" /> &lt;= 0, or <paramref name="Period" /> &gt; <paramref name="Life." /></exception>
		public static decimal DDB(
		  decimal Cost,
		  decimal Salvage,
		  decimal Life,
		  decimal Period,
		  decimal Factor = 2.0M)
		{
			if (Factor <= 0.0M || Salvage < 0.0M || (Period <= 0.0M || Period > Life))
				throw new ArgumentException("Argument_InvalidValue Factor");
			decimal num1;
			if (Cost <= 0.0M)
				num1 = 0.0M;
			else if (Life < 2.0M)
				num1 = Cost - Salvage;
			else if (Life == 2.0M && Period > 1.0M)
				num1 = 0.0M;
			else if (Life == 2.0M && Period <= 1.0M)
				num1 = Cost - Salvage;
			else if (Period <= 1.0M)
			{
				var num2 = Cost * Factor / Life;
				var num3 = Cost - Salvage;
				num1 = num2 <= num3 ? num2 : num3;
			}
			else
			{
				var x = (Life - Factor) / Life;
				var y = Period - 1.0M;
				var num2 = Factor * Cost / Life * (decimal)System.Math.Pow((double)x, (double)y);
				var num3 = Cost * (1.0M - ((decimal)System.Math.Pow((double)x, (double)Period))) - Cost + Salvage;
				if (num3 > 0.0M)
					num2 -= num3;
				num1 = num2 < 0.0M ? 0.0M : num2;
			}
			return num1;
		}
	}
}
