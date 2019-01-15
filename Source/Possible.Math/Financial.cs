using Possible.Math.Enum;
using System;
using System.Threading;

namespace Possible.Math
{
	public class Financial
	{

		/// <summary>Returns a <see langword="Double" /> specifying the interest payment for a given period of an annuity based on periodic, fixed payments and a fixed interest rate.</summary>
		/// <param name="Rate">Required. <see langword="Double" /> specifying interest rate per period. For example, if you get a car loan at an annual percentage rate (APR) of 10 percent and make monthly payments, the rate per period is 0.1/12, or 0.0083.</param>
		/// <param name="Per">Required. <see langword="Double" /> specifying payment period in the range 1 through <paramref name="NPer" />.</param>
		/// <param name="NPer">Required. <see langword="Double" /> specifying total number of payment periods in the annuity. For example, if you make monthly payments on a four-year car loan, your loan has a total of 4 x 12 (or 48) payment periods.</param>
		/// <param name="PV">Required. <see langword="Double" /> specifying present value, or value today, of a series of future payments or receipts. For example, when you borrow money to buy a car, the loan amount is the present value to the lender of the monthly car payments you will make.</param>
		/// <param name="FV">Optional. <see langword="Double" /> specifying future value or cash balance you want after you've made the final payment. For example, the future value of a loan is $0 because that's its value after the final payment. However, if you want to save $50,000 over 18 years for your child's education, then $50,000 is the future value. If omitted, 0 is assumed.</param>
		/// <param name="Due">Optional. Object of type <see cref="T:Microsoft.VisualBasic.DueDate" /> that specifies when payments are due. This argument must be either DueDate.EndOfPeriod if payments are due at the end of the payment period, or DueDate.BegOfPeriod if payments are due at the beginning of the period. If omitted, DueDate.EndOfPeriod is assumed.</param>
		/// <returns>Returns a <see langword="Double" /> specifying the interest payment for a given period of an annuity based on periodic, fixed payments and a fixed interest rate.</returns>
		/// <exception cref="T:System.ArgumentException">
		/// <paramref name="Per" /> &lt;= 0 or <paramref name="Per" /> &gt; <paramref name="NPer" /></exception>
		public static double IPmt(
		  double Rate,
		  double Per,
		  double NPer,
		  double PV,
		  double FV = 0.0,
		  DueDate Due = DueDate.EndOfPeriod)
		{
			var num1 = Due == DueDate.EndOfPeriod ? 1.0 : 2.0;
			if (Per <= 0.0 || Per >= NPer + 1.0)
				throw new ArgumentException("Argument_InvalidValue Per");
			double num2;
			if (Due != DueDate.EndOfPeriod && Per == 1.0)
			{
				num2 = 0.0;
			}
			else
			{
				var Pmt = Financial.PMT_Internal(Rate, NPer, PV, FV, Due);
				if (Due != DueDate.EndOfPeriod)
					PV += Pmt;
				num2 = Financial.FV_Internal(Rate, Per - num1, Pmt, PV, DueDate.EndOfPeriod) * Rate;
			}
			return num2;
		}

		/// <summary>Returns a <see langword="Double" /> specifying the internal rate of return for a series of periodic cash flows (payments and receipts).</summary>
		/// <param name="ValueArray">Required. Array of <see langword="Double" /> specifying cash flow values. The array must contain at least one negative value (a payment) and one positive value (a receipt).</param>
		/// <param name="Guess">Optional. Object specifying value you estimate will be returned by <see langword="IRR" />. If omitted, <paramref name="Guess" /> is 0.1 (10 percent).</param>
		/// <returns>Returns a <see langword="Double" /> specifying the internal rate of return for a series of periodic cash flows (payments and receipts).</returns>
		/// <exception cref="T:System.ArgumentException">Array argument values are invalid or <paramref name="Guess" /> &lt;= -1.</exception>
		public static double IRR(ref double[] ValueArray, double Guess = 0.1)
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
			if (Guess <= -1.0)
				throw new ArgumentException("Argument_InvalidValue Guess");
			if (num1 <= 1)
				throw new ArgumentException("Argument_InvalidValue ValueArray");
			var num2 = ValueArray[0] <= 0.0 ? -ValueArray[0] : ValueArray[0];
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
			var num4 = num2 * 1E-07 * 0.01;
			var Guess1 = Guess;
			var num5 = Financial.OptPV2(ref ValueArray, Guess1);
			var Guess2 = num5 <= 0.0 ? Guess1 - 1E-05 : Guess1 + 1E-05;
			if (Guess2 <= -1.0)
				throw new ArgumentException("Argument_InvalidValue Rate");
			var num6 = Financial.OptPV2(ref ValueArray, Guess2);
			var num7 = 0;
			do
			{
				if (num6 == num5)
				{
					if (Guess2 > Guess1)
						Guess1 -= 1E-05;
					else
						Guess1 += 1E-05;
					num5 = Financial.OptPV2(ref ValueArray, Guess1);
					if (num6 == num5)
						throw new ArgumentException("Argument_InvalidValue");
				}
				var num8 = Guess2;
				var Guess3 = num8 - (num8 - Guess1) * num6 / (num6 - num5);
				if (Guess3 <= -1.0)
					Guess3 = (Guess2 - 1.0) * 0.5;
				var num9 = Financial.OptPV2(ref ValueArray, Guess3);
				var num10 = Guess3 <= Guess2 ? Guess2 - Guess3 : Guess3 - Guess2;
				if ((num9 <= 0.0 ? -num9 : num9) < num4 && num10 < 1E-07)
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

		/// <summary>Returns a <see langword="Double" /> specifying the payment for an annuity based on periodic, fixed payments and a fixed interest rate.</summary>
		/// <param name="Rate">Required. <see langword="Double" /> specifies the interest rate per period. For example, if you get a car loan at an annual percentage rate (APR) of 10 percent and make monthly payments, the rate per period is 0.1/12, or 0.0083.</param>
		/// <param name="NPer">Required. <see langword="Double" /> specifies the total number of payment periods in the annuity. For example, if you make monthly payments on a four-year car loan, your loan has a total of 4 × 12 (or 48) payment periods.</param>
		/// <param name="PV">Required. <see langword="Double" /> specifies the present value (or lump sum) that a series of payments to be paid in the future is worth now. For example, when you borrow money to buy a car, the loan amount is the present value to the lender of the monthly car payments you will make.</param>
		/// <param name="FV">Optional. <see langword="Double" /> specifying future value or cash balance you want after you have made the final payment. For example, the future value of a loan is $0 because that is its value after the final payment. However, if you want to save $50,000 during 18 years for your child's education, then $50,000 is the future value. If omitted, 0 is assumed.</param>
		/// <param name="Due">Optional. Object of type <see cref="T:Microsoft.VisualBasic.DueDate" /> that specifies when payments are due. This argument must be either DueDate.EndOfPeriod if payments are due at the end of the payment period, or DueDate.BegOfPeriod if payments are due at the beginning of the period. If omitted, DueDate.EndOfPeriod is assumed.</param>
		/// <returns>Returns a <see langword="Double" /> specifying the payment for an annuity based on periodic, fixed payments and a fixed interest rate.</returns>
		/// <exception cref="T:System.ArgumentException">
		/// <paramref name="NPer" /> = 0.</exception>
		public static double Pmt(double Rate, double NPer, double PV, double FV = 0.0, DueDate Due = DueDate.EndOfPeriod)
		{
			return Financial.PMT_Internal(Rate, NPer, PV, FV, Due);
		}

		private static double PMT_Internal(
		  double Rate,
		  double NPer,
		  double PV,
		  double FV = 0.0,
		  DueDate Due = DueDate.EndOfPeriod)
		{
			if (NPer == 0.0)
				throw new ArgumentException("Argument_InvalidValue NPer");
			double num1;
			if (Rate == 0.0)
			{
				num1 = (-FV - PV) / NPer;
			}
			else
			{
				var num2 = Due == DueDate.EndOfPeriod ? 1.0 : 1.0 + Rate;
				var num3 = System.Math.Pow(Rate + 1.0, NPer);
				num1 = (-FV - PV * num3) / (num2 * (num3 - 1.0)) * Rate;
			}
			return num1;
		}

		/// <summary>Returns a <see langword="Double" /> specifying the principal payment for a given period of an annuity based on periodic fixed payments and a fixed interest rate.</summary>
		/// <param name="Rate">Required. <see langword="Double" /> specifies the interest rate per period. For example, if you get a car loan at an annual percentage rate (APR) of 10 percent and make monthly payments, the rate per period is 0.1/12, or 0.0083.</param>
		/// <param name="Per">Required. <see langword="Double" /> specifies the payment period in the range 1 through <paramref name="NPer" />.</param>
		/// <param name="NPer">Required. <see langword="Double" /> specifies the total number of payment periods in the annuity. For example, if you make monthly payments on a four-year car loan, your loan has a total of 4 x 12 (or 48) payment periods.</param>
		/// <param name="PV">Required. <see langword="Double" /> specifies the current value of a series of future payments or receipts. For example, when you borrow money to buy a car, the loan amount is the present value to the lender of the monthly car payments you will make.</param>
		/// <param name="FV">Optional. <see langword="Double" /> specifying future value or cash balance you want after you have made the final payment. For example, the future value of a loan is $0 because that is its value after the final payment. However, if you want to save $50,000 over 18 years for your child's education, then $50,000 is the future value. If omitted, 0 is assumed.</param>
		/// <param name="Due">Optional. Object of type <see cref="T:Microsoft.VisualBasic.DueDate" /> that specifies when payments are due. This argument must be either DueDate.EndOfPeriod if payments are due at the end of the payment period, or DueDate.BegOfPeriod if payments are due at the beginning of the period. If omitted, DueDate.EndOfPeriod is assumed.</param>
		/// <returns>Returns a <see langword="Double" /> specifying the principal payment for a given period of an annuity based on periodic fixed payments and a fixed interest rate.</returns>
		/// <exception cref="T:System.ArgumentException">
		/// <paramref name="Per" /> &lt;=0 or <paramref name="Per" /> &gt; <paramref name="NPer" />.</exception>
		public static double PPmt(
		  double Rate,
		  double Per,
		  double NPer,
		  double PV,
		  double FV = 0.0,
		  DueDate Due = DueDate.EndOfPeriod)
		{
			if (Per <= 0.0 || Per >= NPer + 1.0)
				throw new ArgumentException("PPMT_PerGT0AndLTNPer");
			return Financial.PMT_Internal(Rate, NPer, PV, FV, Due) - Financial.IPmt(Rate, Per, NPer, PV, FV, Due);
		}


		private static double OptPV2(ref double[] ValueArray, double Guess = 0.1)
		{
			var index1 = 0;
			var upperBound = ValueArray.GetUpperBound(0);
			var num1 = 0.0;
			var num2 = 1.0 + Guess;
			while (index1 <= upperBound && ValueArray[index1] == 0.0)
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

		private static double FV_Internal(
			double Rate,
			double NPer,
			double Pmt,
			double PV = 0.0,
			DueDate Due = DueDate.EndOfPeriod)
		{
			double num1;
			if (Rate == 0.0)
			{
				num1 = -PV - Pmt * NPer;
			}
			else
			{
				var num2 = Due == DueDate.EndOfPeriod ? 1.0 : 1.0 + Rate;
				var num3 = System.Math.Pow(1.0 + Rate, NPer);
				num1 = -PV * num3 - Pmt / Rate * num2 * (num3 - 1.0);
			}
			return num1;
		}

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
				var Pmt = Financial.PMT_Internal(Rate, NPer, PV, FV, Due);
				if (Due != DueDate.EndOfPeriod)
					PV += Pmt;
				num2 = Financial.FV_Internal(Rate, Per - num1, Pmt, PV, DueDate.EndOfPeriod) * Rate;
			}
			return num2;
		}

		/// <summary>Returns a <see langword="decimal" /> specifying the internal rate of return for a series of periodic cash flows (payments and receipts).</summary>
		/// <param name="ValueArray">Required. Array of <see langword="decimal" /> specifying cash flow values. The array must contain at least one negative value (a payment) and one positive value (a receipt).</param>
		/// <param name="Guess">Optional. Object specifying value you estimate will be returned by <see langword="IRR" />. If omitted, <paramref name="Guess" /> is 0.1 (10 percent).</param>
		/// <returns>Returns a <see langword="decimal" /> specifying the internal rate of return for a series of periodic cash flows (payments and receipts).</returns>
		/// <exception cref="T:System.ArgumentException">Array argument values are invalid or <paramref name="Guess" /> &lt;= -1.</exception>
		public static decimal IRR(ref decimal[] ValueArray, decimal Guess = 0.1M)
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
			var num5 = Financial.OptPV2(ref ValueArray, Guess1);
			var Guess2 = num5 <= 0.0M ? Guess1 - 1E-05M : Guess1 + 1E-05M;
			if (Guess2 <= -1.0M)
				throw new ArgumentException("Argument_InvalidValue Rate");
			var num6 = Financial.OptPV2(ref ValueArray, Guess2);
			var num7 = 0;
			do
			{
				if (num6 == num5)
				{
					if (Guess2 > Guess1)
						Guess1 -= 1E-05M;
					else
						Guess1 += 1E-05M;
					num5 = Financial.OptPV2(ref ValueArray, Guess1);
					if (num6 == num5)
						throw new ArgumentException("Argument_InvalidValue");
				}
				var num8 = Guess2;
				var Guess3 = num8 - (num8 - Guess1) * num6 / (num6 - num5);
				if (Guess3 <= -1.0M)
					Guess3 = (Guess2 - 1.0M) * 0.5M;
				var num9 = Financial.OptPV2(ref ValueArray, Guess3);
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
		public static decimal Pmt(decimal Rate, decimal NPer, decimal PV, decimal FV = 0.0M, DueDate Due = DueDate.EndOfPeriod)
		{
			return Financial.PMT_Internal(Rate, NPer, PV, FV, Due);
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
			return Financial.PMT_Internal(Rate, NPer, PV, FV, Due) - Financial.IPmt(Rate, Per, NPer, PV, FV, Due);
		}


		private static decimal OptPV2(ref decimal[] ValueArray, decimal Guess = 0.1M)
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
	}
}
