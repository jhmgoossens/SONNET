// Copyright (C) 2011, Jan-Willem Goossens 
// All Rights Reserved.
// This code is licensed under the terms of the Eclipse Public License (EPL).

using System;
using System.Diagnostics;
using System.Runtime;
using System.Collections.Generic;
using System.Linq;

using Sonnet;
using COIN;
using System.IO;
using System.Reflection;

namespace SonnetTest
{
    public class MathExtension
    {
        public static double Epsilon
        {
            get { return 1e-5; }
        }
        public static int CompareDouble(double a, double b)
        {
            if (a < b - MathExtension.Epsilon) return -1;
            if (a > b + MathExtension.Epsilon) return 1;

            return 0;
        }
    }
    public class SonnetTest
    {
        /// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
#if (DYNAMIC_LOADING)
            string assemblyDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (File.Exists(Path.Combine(assemblyDir, "SonnetWrapper.proxy.dll"))
                || !File.Exists(Path.Combine(assemblyDir, "SonnetWrapper.x86.dll"))
                || !File.Exists(Path.Combine(assemblyDir, "SonnetWrapper.x64.dll")))
            {
                throw new InvalidOperationException("Found SonnetWrapper.proxy.dll which cannot exist. "
                    + "Must instead have SonnetWrapper.x86.dll and SonnetWrapper.x64.dll. Check your build settings.");
            }

            AppDomain.CurrentDomain.AssemblyResolve += (_, e) =>
            {
                if (e.Name.StartsWith("SonnetWrapper.proxy,", StringComparison.OrdinalIgnoreCase))
                {
                    string fileName = Path.Combine(assemblyDir,
                        string.Format("SonnetWrapper.{0}.dll", (IntPtr.Size == 4) ? "x86" : "x64"));
                    return Assembly.LoadFile(fileName);
                }
                return null;
            };
#endif
            SonnetTest test = new SonnetTest();

			test.TestMain(args);
		}

        public SolverType solverType;

        public string FindBinParent(string directory)
        {
            if (directory == null || directory.Length == 0) return string.Empty;

            string parent = System.IO.Path.GetDirectoryName(directory);
            if (parent == null || parent.Length == 0) return string.Empty;

#if (!VS2003) // must be defined if used in VS2003
            if (parent.EndsWith("bin", StringComparison.CurrentCultureIgnoreCase)) return parent;
#else
			if (parent.EndsWith("bin")) return parent;
#endif
            else return FindBinParent(parent);
        }
        public void Assert(bool test)
        {
            if (!test)
            {
                if (!System.Diagnostics.Debugger.IsAttached) System.Diagnostics.Debugger.Launch();

                System.Diagnostics.Debugger.Break();
                throw new ApplicationException("Assertion failed");
            }
        }
        public SonnetTest()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private static System.Reflection.FieldInfo GetNumberOfVariablesOfVariableClass()
        {
            return typeof(Variable).GetField("numberOfVariables", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
        }

        private static System.Reflection.FieldInfo GetNumberOfConstraintsOfConstraintClass()
        {
            return typeof(Constraint).GetField("numberOfConstraints", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
        }

        private static System.Reflection.FieldInfo GetNumberOfObjectivesOfObjectiveClass()
        {
            return typeof(Objective).GetField("numberOfObjectives", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
        }

        public void TestMain(string[] args)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.GetCultureInfo("en-GB");

            Model model = new Model();
            SonnetLog.Default.LogLevel = 1;

            // dont test VolSolver..
            foreach (SolverType lpSolver in Enum.GetValues(typeof(SolverType)))
            {
                this.solverType = lpSolver;
                if (!Solver.CanCreateSolver(solverType)) continue;

                try
                {
                    SonnetTest0();
                    SonnetTest0b();
                    SonnetTest1();
                    SonnetTest2();
                    SonnetTest3();
                    SonnetTest4();
                    SonnetTest5();
                    SonnetTest6();
                    SonnetTest7();
                    SonnetTest8();
                    SonnetTest9();
                    SonnetTest10();
#if (CONSTRAINT_SET_COEF)
                    SonnetTest11();
                    SonnetTest12();
                    SonnetTest13();
#endif
                    SonnetTest14();
                    SonnetTest15();
                    SonnetTest16();
                    SonnetTest17();
                    SonnetTest18();
                    SonnetTest19();
                    SonnetTest20();
                    SonnetTest21();
                    SonnetTest22();
                    SonnetTest23();
                    SonnetTest24();
                    SonnetTest25();
                    SonnetTest26();
                    SonnetTest26b();
                    SonnetTest27();
                    SonnetTest28();
                    SonnetTest31();
                    SonnetTest32();
                    SonnetTest33();
                    SonnetTest34();
                    SonnetTest35();
                    SonnetTest36();
                    SonnetTest37();
                    SonnetTest38();
                    SonnetTest39();

                    // do these two stress tests last..
                    SonnetTest29();
                    SonnetTest30(); 
                }
                catch
                {
                    if (!System.Diagnostics.Debugger.IsAttached) System.Diagnostics.Debugger.Launch();
                    System.Diagnostics.Debugger.Break();
                }
            }
            model.Clear();
            model = null;
            System.GC.Collect();

            Console.WriteLine("\n\nUnit Test finished. Press enter to finish.");
            Console.ReadLine();
        }


        public void SonnetTest1()
        {
            Console.WriteLine("SonnetTest1");

            Model model = new Model();
            Solver solver = new Solver(model, solverType);

            Variable x0 = new Variable("x0");
            Variable x1 = new Variable("x1");

            RangeConstraint con0 = -model.Infinity <= x0 * 2 + x1 * 1 <= 10;
            RangeConstraint con1 = -model.Infinity <= x0 * 1 + x1 * 3 <= 15;
            model.Add(con0, "con0");
            model.Add(con1, "con1");

            Objective obj = model.Objective = x0 * 3 + x1 * 1;
            x0.Lower = 0;
            x1.Lower = 0;

            x0.Upper = model.Infinity;
            x1.Upper = model.Infinity;

            model.ObjectiveSense = ObjectiveSense.Maximise;

            solver.Solve();

            x0.Upper = 0.0;
            x1.Upper = 0.0;

            x0.Upper = model.Infinity;
            x1.Upper = model.Infinity;

            solver.Solve();

            Assert(solver.IsProvenOptimal);
            Assert(!solver.IsProvenPrimalInfeasible);
            Assert(!solver.IsProvenDualInfeasible);

            Assert(MathExtension.CompareDouble(x0.Value, 5.0) == 0 &&
                MathExtension.CompareDouble(x1.Value, 0.0) == 0);

            Assert(MathExtension.CompareDouble(con0.Value, 10.0) == 0 &&
                MathExtension.CompareDouble(solver.GetConstraint("con1").Value, 5.0) == 0);

            model.Objective.SetCoefficient(x0, 1.0);
            obj.SetCoefficient(x1, 1.0);

            solver.Resolve();

            Assert(solver.IsProvenOptimal);
            Assert(!solver.IsProvenPrimalInfeasible);
            Assert(!solver.IsProvenDualInfeasible);

            Assert(MathExtension.CompareDouble(x0.Value, 3.0) == 0 &&
                MathExtension.CompareDouble(x1.Value, 4.0) == 0);

            Assert(MathExtension.CompareDouble(solver.GetConstraint("con0").Value, 10.0) == 0 &&
                MathExtension.CompareDouble(solver.GetConstraint("con1").Value, 15.0) == 0);
        }


        public void SonnetTest2()
        {
            Console.WriteLine("SonnetTest2");

            Model model = new Model();
            Solver solver = new Solver(model, solverType);

            Variable x0 = new Variable("x0");
            Variable x1 = new Variable("x1");

            Constraint r0 = x0 * 2 + x1 * 1 <= 10.0;
            Constraint r1 = x0 * 1 + x1 * 3 <= 15.0;
            Constraint r2 = 1 <= x0 * 1 + x1 * 1 <= model.Infinity;
            Objective obj = model.Objective = (3.0 * x0 + 1.0 * x1);
            model.ObjectiveSense = ObjectiveSense.Maximise;

            model.Add(r0);
            model.Add(r1);
            model.Add(r2);

            solver.Solve();

            //Console.WriteLine(model);
            Assert(solver.IsProvenOptimal);
            Assert(!solver.IsProvenPrimalInfeasible);
            Assert(!solver.IsProvenDualInfeasible);

            Assert(MathExtension.CompareDouble(x0.Value, 5.0) == 0 &&
                MathExtension.CompareDouble(x1.Value, 0.0) == 0);

            Assert(MathExtension.CompareDouble(r0.Value, 10.0) == 0 &&
                MathExtension.CompareDouble(r1.Value, 5.0) == 0 &&
                MathExtension.CompareDouble(r2.Value, 5.0) == 0);

            obj.SetCoefficient(x0, 1.0);
            obj.SetCoefficient(x1, 1.0);

            solver.Resolve();

            Assert(solver.IsProvenOptimal);
            Assert(!solver.IsProvenPrimalInfeasible);
            Assert(!solver.IsProvenDualInfeasible);

            Assert(MathExtension.CompareDouble(x0.Value, 3.0) == 0 &&
                MathExtension.CompareDouble(x1.Value, 4.0) == 0);

            Assert(MathExtension.CompareDouble(r0.Value, 10.0) == 0 &&
                MathExtension.CompareDouble(r1.Value, 15.0) == 0 &&
                MathExtension.CompareDouble(r2.Value, 7.0) == 0);
        }

        public void SonnetTest3()
        {
            Console.WriteLine("SonnetTest3");

            Model model = new Model();
            Solver solver = new Solver(model, solverType);

            Variable x0 = new Variable("x0", 0.0, 10.0);
            Variable x1 = new Variable("x1", 0.0, 10.0);

            RangeConstraint con0 = 0.0 <= x0 * 2 + x1 * 1 <= 10;
            RangeConstraint con1 = 0.0 <= x0 * 1 + x1 * 3 <= 15;

            model.Add(con0, "con0");

            solver.Generate();

            model.Add(con1, "con1");

            Objective obj = model.Objective = (x0 * 3 + x1 * 1);

            model.ObjectiveSense = ObjectiveSense.Maximise;

            solver.Solve();

            Assert(solver.IsProvenOptimal);
            Assert(!solver.IsProvenPrimalInfeasible);
            Assert(!solver.IsProvenDualInfeasible);

            Assert(MathExtension.CompareDouble(x0.Value, 5.0) == 0 &&
                MathExtension.CompareDouble(x1.Value, 0.0) == 0);

            Assert(MathExtension.CompareDouble(model.GetConstraint("con0").Value, 10.0) == 0 &&
                MathExtension.CompareDouble(model.GetConstraint("con1").Value, 5.0) == 0);

            model.Objective.SetCoefficient(x0, 1.0);
            obj.SetCoefficient(x1, 1.0);

            solver.Resolve();


            Assert(solver.IsProvenOptimal);
            Assert(!solver.IsProvenPrimalInfeasible);
            Assert(!solver.IsProvenDualInfeasible);

            Assert(MathExtension.CompareDouble(x0.Value, 3.0) == 0 &&
                MathExtension.CompareDouble(x1.Value, 4.0) == 0);

            Assert(MathExtension.CompareDouble(model.GetConstraint("con0").Value, 10.0) == 0 &&
                MathExtension.CompareDouble(model.GetConstraint("con1").Value, 15.0) == 0);
        }

        public void SonnetTest4()
        {
            Console.WriteLine("SonnetTest4");

            Model model = new Model();
            Solver solver = new Solver(model, solverType);

            Variable x0 = new Variable("x0");
            Variable x1 = new Variable("x1");

            RangeConstraint r0 = -model.Infinity <= x0 * 2 + x1 * 1 <= 10;
            RangeConstraint r1 = -model.Infinity <= x0 * 1 + x1 * 3 <= 15;

            model.Add(r0);

            solver.Generate();

            model.Add(r1);

            Objective obj = model.Objective = (x0 * 3 + x1 * 1);
            model.ObjectiveSense = ObjectiveSense.Maximise;

            solver.Solve();

            Assert(solver.IsProvenOptimal);
            Assert(!solver.IsProvenPrimalInfeasible);
            Assert(!solver.IsProvenDualInfeasible);

            Assert(MathExtension.CompareDouble(x0.Value, 5.0) == 0 &&
                MathExtension.CompareDouble(x1.Value, 0.0) == 0);

            Assert(MathExtension.CompareDouble(r0.Value, 10.0) == 0 &&
                MathExtension.CompareDouble(r1.Value, 5.0) == 0);

            model.Objective.SetCoefficient(x0, 1.0);
            obj.SetCoefficient(x1, 1.0);

            solver.Resolve();

            Assert(solver.IsProvenOptimal);
            Assert(!solver.IsProvenPrimalInfeasible);
            Assert(!solver.IsProvenDualInfeasible);

            Assert(MathExtension.CompareDouble(x0.Value, 3.0) == 0 &&
                MathExtension.CompareDouble(x1.Value, 4.0) == 0);

            Assert(MathExtension.CompareDouble(r0.Value, 10.0) == 0 &&
                MathExtension.CompareDouble(r1.Value, 15.0) == 0);
        }

        public void SonnetTest5()
        {
            Console.WriteLine("SonnetTest5");

            Model model = new Model();
            Solver solver = new Solver(model, solverType);

            Variable x0 = new Variable("x0", -model.Infinity, model.Infinity);
            Variable x1 = new Variable("x1", -model.Infinity, model.Infinity);

            Objective obj = model.Objective = x0 * 3 + x1 * 1;

            RangeConstraint r0 = -model.Infinity <= x0 * 2 + x1 * 1 <= 10;
            RangeConstraint r1 = -model.Infinity <= x0 * 1 + x1 * 3 <= 15;

            model.Add(r0);
            model.Add(r1);

            model.ObjectiveSense = ObjectiveSense.Maximise;

            solver.Solve();

            Assert(!solver.IsProvenOptimal);
            Assert(!solver.IsProvenPrimalInfeasible);
            Assert(solver.IsProvenDualInfeasible);

            obj.SetCoefficient(x0, 1.0);
            obj.SetCoefficient(x1, 1.0);

            solver.Resolve();

            Assert(solver.IsProvenOptimal);
            Assert(!solver.IsProvenPrimalInfeasible);
            Assert(!solver.IsProvenDualInfeasible);

            Assert(MathExtension.CompareDouble(x0.Value, 3.0) == 0 &&
                MathExtension.CompareDouble(x1.Value, 4.0) == 0);

            Assert(MathExtension.CompareDouble(r0.Value, 10.0) == 0 &&
                MathExtension.CompareDouble(r1.Value, 15.0) == 0);
        }

        public void SonnetTest6()
        {
            Console.WriteLine("SonnetTest6");

            Model model = new Model();
            Solver solver = new Solver(model, solverType);

            Variable x0 = new Variable("x0", 0, model.Infinity);
            Variable x1 = new Variable("x1", 0, model.Infinity);

            Objective obj = new Objective();
            obj.SetCoefficient(x0, 3.0);
            obj.SetCoefficient(x1, 1.0);
            model.Objective = obj;

            Constraint r0 = x0 * 2 + x1 * 1 <= 10;
            Constraint r1 = x0 * 1 + x1 * 3 <= 15;

            model.Add(r0);
            model.Add(r1);

            model.ObjectiveSense = ObjectiveSense.Maximise;

            solver.Solve();

            Assert(solver.IsProvenOptimal);
            Assert(!solver.IsProvenPrimalInfeasible);
            Assert(!solver.IsProvenDualInfeasible);

            Assert(MathExtension.CompareDouble(x0.Value, 5.0) == 0 &&
                MathExtension.CompareDouble(x1.Value, 0.0) == 0);

            Assert(MathExtension.CompareDouble(r0.Value, 10.0) == 0 &&
                MathExtension.CompareDouble(r1.Value, 5.0) == 0);

            obj.SetCoefficient(x0, 1.0);
            obj.SetCoefficient(x1, 1.0);

            solver.Resolve();

            Assert(solver.IsProvenOptimal);
            Assert(!solver.IsProvenPrimalInfeasible);
            Assert(!solver.IsProvenDualInfeasible);

            Assert(MathExtension.CompareDouble(x0.Value, 3.0) == 0 &&
                MathExtension.CompareDouble(x1.Value, 4.0) == 0);

            Assert(MathExtension.CompareDouble(r0.Value, 10.0) == 0 &&
                MathExtension.CompareDouble(r1.Value, 15.0) == 0);
        }

        public void SonnetTest7()
        {
            Console.WriteLine("SonnetTest7");

            Model model = new Model();
            Solver solver = new Solver(model, solverType);

            Variable x0 = new Variable("x0", 4, model.Infinity);
            Variable x1 = new Variable("x1", 3, model.Infinity);

            Objective obj = model.Objective = 3 * x0 + 1 * x1;

            Constraint r0 = x0 * 2 + x1 * 1 <= 10;
            Constraint r1 = x0 * 1 + x1 * 3 <= 15;

            model.Add(r0);
            model.Add(r1);

            model.ObjectiveSense = ObjectiveSense.Maximise;

            solver.Solve();

            Assert(!solver.IsProvenOptimal);
            Assert(solver.IsProvenPrimalInfeasible);

            obj.SetCoefficient(x0, 1.0);
            obj.SetCoefficient(x1, 1.0);

            solver.Resolve();

            Assert(!solver.IsProvenOptimal);
            Assert(solver.IsProvenPrimalInfeasible);
        }

        public void SonnetTest8()
        {
            Console.WriteLine("SonnetTest8");

            Model model = new Model();
            Solver solver = new Solver(model, solverType);

            Variable x0 = new Variable("x0", Double.MinValue, Double.MaxValue);
            Variable x1 = new Variable("x1", Double.MinValue, Double.MaxValue);

            Objective obj = model.Objective = (3 * x0 + 1 * x1);

            RangeConstraint r0 = 0.0 <= x0 * 2 + x1 * 1 <= 10;
            RangeConstraint r1 = 0.0 <= x0 * 1 + x1 * 3 <= 15;

            model.Add(r0);
            model.Add(r1);

            model.ObjectiveSense = ObjectiveSense.Maximise;

            solver.Solve();

            Assert(solver.IsProvenOptimal);
            Assert(!solver.IsProvenPrimalInfeasible);
            Assert(!solver.IsProvenDualInfeasible);

            Assert(MathExtension.CompareDouble(x0.Value, 6.0) == 0 &&
                MathExtension.CompareDouble(x1.Value, -2.0) == 0);

            Assert(MathExtension.CompareDouble(r0.Value, 10.0) == 0 &&
                MathExtension.CompareDouble(r1.Value, 0.0) == 0);

            obj.SetCoefficient(x0, 1.0);
            obj.SetCoefficient(x1, 1.0);

            solver.Resolve();


            Assert(solver.IsProvenOptimal);
            Assert(!solver.IsProvenPrimalInfeasible);
            Assert(!solver.IsProvenDualInfeasible);

            Assert(MathExtension.CompareDouble(x0.Value, 3.0) == 0 &&
                MathExtension.CompareDouble(x1.Value, 4.0) == 0);

            Assert(MathExtension.CompareDouble(r0.Value, 10.0) == 0 &&
                MathExtension.CompareDouble(r1.Value, 15.0) == 0);
        }

        public void SonnetTest9()
        {
            Console.WriteLine("SonnetTest9");

            Model model = new Model();
            Solver solver = new Solver(model, solverType);

            Variable x0 = new Variable("x0", Double.MinValue, Double.MaxValue);
            Variable x1 = new Variable("x1", Double.MinValue, Double.MaxValue);

            Objective obj = model.Objective = (3 * x0 + 1 * x1);

            RangeConstraint r0 = 0.0 <= x0 * 2 + x1 * 1 <= 10;
            RangeConstraint r1 = 0.0 <= x0 * 1 + x1 * 3 <= 15;
            Constraint r2 = 1 * x0 + 4 * x1 >= 12;

            model.Add(r0);
            model.Add(r1);
            model.Add(r2);

            model.ObjectiveSense = ObjectiveSense.Maximise;

            solver.Solve();

            Assert(solver.IsProvenOptimal);
            Assert(!solver.IsProvenPrimalInfeasible);
            Assert(!solver.IsProvenDualInfeasible);

            Assert(MathExtension.CompareDouble(x0.Value, 4.0) == 0 &&
                MathExtension.CompareDouble(x1.Value, 2.0) == 0);

            Assert(MathExtension.CompareDouble(r0.Value, 10.0) == 0 &&
                MathExtension.CompareDouble(r1.Value, 10.0) == 0 &&
                MathExtension.CompareDouble(r2.Value, 12.0) == 0);

            obj.SetCoefficient(x0, 1.0);
            obj.SetCoefficient(x1, 1.0);

            solver.Resolve();

            Assert(solver.IsProvenOptimal);
            Assert(!solver.IsProvenPrimalInfeasible);
            Assert(!solver.IsProvenDualInfeasible);

            Assert(MathExtension.CompareDouble(x0.Value, 3.0) == 0 &&
                MathExtension.CompareDouble(x1.Value, 4.0) == 0);

            Assert(MathExtension.CompareDouble(r0.Value, 10.0) == 0 &&
                MathExtension.CompareDouble(r1.Value, 15.0) == 0 &&
                MathExtension.CompareDouble(r2.Value, 19.0) == 0);
        }

        public void SonnetTest10()
        {
            Console.WriteLine("SonnetTest10");

            Model model = new Model();
            Solver solver = new Solver(model, solverType);

            Variable x0 = new Variable("x0");
            Variable x1 = new Variable("x1");

            Objective obj = model.Objective = (3 * x0 + 1 * x1);

            Constraint r0 = 4 * x0 + 2 * x1 <= 20;
            Constraint r1 = 1 * x0 + 3 * x1 <= 15;

            model.Add(r0);
            model.Add(r1);

            model.ObjectiveSense = ObjectiveSense.Maximise;

            solver.Solve();


            Assert(solver.IsProvenOptimal);
            Assert(!solver.IsProvenPrimalInfeasible);
            Assert(!solver.IsProvenDualInfeasible);

            Assert(MathExtension.CompareDouble(x0.Value, 5.0) == 0 &&
                MathExtension.CompareDouble(x1.Value, 0.0) == 0);

            Assert(MathExtension.CompareDouble(r0.Value, 20.0) == 0 &&
                MathExtension.CompareDouble(r1.Value, 5.0) == 0);

            obj.SetCoefficient(x0, 1);
            obj.SetCoefficient(x1, 1);

            solver.Resolve();


            Assert(solver.IsProvenOptimal);
            Assert(!solver.IsProvenPrimalInfeasible);
            Assert(!solver.IsProvenDualInfeasible);

            Assert(MathExtension.CompareDouble(x0.Value, 3.0) == 0 &&
                MathExtension.CompareDouble(x1.Value, 4.0) == 0);

            Assert(MathExtension.CompareDouble(r0.Value, 20.0) == 0 &&
                MathExtension.CompareDouble(r1.Value, 15.0) == 0);
        }

#if (CONSTRAINT_SET_COEF)
        public void SonnetTest11()
        {
            Console.WriteLine("SonnetTest11");

            Model model = new Model();
            Solver solver = new Solver(model, solverType);

            Variable x0 = new Variable("x0");
            Variable x1 = new Variable("x1");

            x0.Lower = Double.MinValue;
            x1.Lower = Double.MinValue;

            Objective obj = model.Objective = (3 * x0 + 1 * x1);

            RangeConstraint r0 = Double.MinValue <= x0 + x0 + x1 <= 10;
            RangeConstraint r1 = Double.MinValue <= x0 * 1 + x1 + x1 <= 15;

            model.Add(r0);
            model.Add(r1);

            r1.SetCoefficient(x1, 3.0);

            model.ObjectiveSense = ObjectiveSense.Maximise;

            solver = new Solver(model, solverType);
            solver.Solve();


            Assert(!solver.IsProvenOptimal);
            Assert(!solver.IsProvenPrimalInfeasible);
            Assert(solver.IsProvenDualInfeasible);

            r0.Lower = 0.0;
            r1.Lower = 0.0;

            solver.Resolve();


            Assert(solver.IsProvenOptimal);
            Assert(!solver.IsProvenPrimalInfeasible);
            Assert(!solver.IsProvenDualInfeasible);

            Assert(MathExtension.CompareDouble(x0.Value, 6.0) == 0 &&
                MathExtension.CompareDouble(x1.Value, -2.0) == 0);

            Assert(MathExtension.CompareDouble(r0.Value, 10.0) == 0 &&
                MathExtension.CompareDouble(r1.Value, 0.0) == 0);

            //model.SetConstraintLower(r0, Double.MinValue);
            //model.SetConstraintLower(r1, Double.MinValue);
            //model.SetConstraintUpper(r0, 20.0);
            //model.SetConstraintUpper(r1, 15.0);
            //model.SetCoefficient(r0, x0, 4.0);
            //model.SetCoefficient(r0, x1, 1.0);
            //model.SetCoefficient(r1, x0, 2.0);
            //model.SetCoefficient(r1, x1, 3.0);
            r0.Lower = Double.MinValue;
            r1.Lower = Double.MinValue;
            r0.Upper = 20.0;
            r1.Upper = 15.0;
            r0.SetCoefficient(x0, 4.0);
            r0.SetCoefficient(x1, 1.0);
            r1.SetCoefficient(x0, 2.0);
            r1.SetCoefficient(x1, 3.0);

            solver.Resolve();


            Console.WriteLine(solver.ToString());
            Console.WriteLine(solver.ToSolutionString());

            Assert(solver.IsProvenOptimal);
            Assert(!solver.IsProvenPrimalInfeasible);
            Assert(!solver.IsProvenDualInfeasible);

            Assert(MathExtension.CompareDouble(x0.Value, 4.5) == 0 &&
                MathExtension.CompareDouble(x1.Value, 2.0) == 0);

            Assert(MathExtension.CompareDouble(r0.Value, 20.0) == 0 &&
                MathExtension.CompareDouble(r1.Value, 15.0) == 0);
        }

        public void SonnetTest12()
        {
            Console.WriteLine("SonnetTest12");

            Model model = new Model();
            Solver solver = new Solver(model, solverType);

            Variable x0 = new Variable("x0");
            Variable x1 = new Variable("x1");

            Objective obj = model.Objective = (3 * x0 + 1 * x1);

            RangeConstraint r0 = 10 <= 4 * x0 - 30 <= -10;
            Constraint r1 = 2 * x0 + 3 * x1 <= 15 + x0;

            model.Add(r0);
            solver.Generate();

            Assert(MathExtension.CompareDouble(r0.Lower, 40.0) == 0);

            r0.Lower = 0.0;
            r0.SetCoefficient(x1, 2.0);
            model.Add(r1);

            model.ObjectiveSense = ObjectiveSense.Maximise;

            solver.Solve();


            Assert(solver.IsProvenOptimal);
            Assert(!solver.IsProvenPrimalInfeasible);
            Assert(!solver.IsProvenDualInfeasible);

            Assert(MathExtension.CompareDouble(x0.Value, 5.0) == 0 &&
                MathExtension.CompareDouble(x1.Value, 0.0) == 0);

            Assert(MathExtension.CompareDouble(r0.Value, 20.0) == 0 &&
                MathExtension.CompareDouble(r1.Value, 5.0) == 0);

            model.Objective.SetCoefficient(x0, 1);
            model.Objective.SetCoefficient(x1, 1);

            solver.Resolve();


            Assert(solver.IsProvenOptimal);
            Assert(!solver.IsProvenPrimalInfeasible);
            Assert(!solver.IsProvenDualInfeasible);

            Assert(MathExtension.CompareDouble(x0.Value, 3.0) == 0 &&
                MathExtension.CompareDouble(x1.Value, 4.0) == 0);

            Assert(MathExtension.CompareDouble(r0.Value, 20.0) == 0 &&
                MathExtension.CompareDouble(r1.Value, 15.0) == 0);
        }

        public void SonnetTest13()
        {
            Console.WriteLine("SonnetTest13");

            Model model = new Model();
            Solver solver = new Solver(model, solverType);

            Variable x0 = new Variable("x0");
            Variable x1 = new Variable("x1");

            Objective obj = model.Objective = (3 * x0 + 1 * x1);

            RangeConstraint r0 = 10 <= 4 * x0 - 30 <= -10;
            Constraint r1 = 2 * x0 + 3 * x1 <= 15 + x0;

            model.Add(r0);
            solver.Generate();

            Assert(MathExtension.CompareDouble(r0.Lower, 40.0) == 0);
            r0.Lower = 0.0;
            r0.SetCoefficient(x1, 2.0);
            model.Add(r1);

            model.ObjectiveSense = ObjectiveSense.Maximise;

            Assert(MathExtension.CompareDouble(r0.GetCoefficient(x0), 4.0) == 0);
            Assert(MathExtension.CompareDouble(r0.GetCoefficient(x1), 2.0) == 0);

            solver.Solve();


            Assert(solver.IsProvenOptimal);
            Assert(!solver.IsProvenPrimalInfeasible);
            Assert(!solver.IsProvenDualInfeasible);

            Assert(MathExtension.CompareDouble(x0.Value, 5.0) == 0 &&
                MathExtension.CompareDouble(x1.Value, 0.0) == 0);

            Assert(MathExtension.CompareDouble(r0.Value, 20.0) == 0 &&
                MathExtension.CompareDouble(r1.Value, 5.0) == 0);


            RangeConstraint r2 = -Double.MaxValue <= 4 * x0 + x1 - 20 <= 0;
            model.Add(r2);

            solver.Generate();

            Assert(MathExtension.CompareDouble(r2.Lower, -Double.MaxValue) == 0);
            Assert(MathExtension.CompareDouble(r2.Upper, 20.0) == 0);

            RangeConstraint r3 = -40.0 <= -4 * x0 - x1 - 20.0 <= Double.MaxValue;
            model.Add(r3);

            solver.Generate();

            Assert(MathExtension.CompareDouble(r3.Lower, -20.0) == 0);
            Assert(MathExtension.CompareDouble(r3.Upper, Double.MaxValue) == 0);


            solver.Solve();


            Assert(MathExtension.CompareDouble(x0.Value, 5.0) == 0 &&
                MathExtension.CompareDouble(x1.Value, 0.0) == 0);

            Assert(MathExtension.CompareDouble(r0.Value, 20.0) == 0 &&
                MathExtension.CompareDouble(r1.Value, 5.0) == 0);
        }
#endif // CONSTRAINT_SET_COEF

        public void SonnetTest14()
        {
            Console.WriteLine("SonnetTest14");

            /*
            With this matrix we have a primal/dual infeas problem. Leaving the first
            row makes it primal feas, leaving the first col makes it dual feas. 
            (JWG but leaving the first col out of the primal representation will
            make the primal unfeasible..)
            All vars are >= 0

            obj:-1  2 -3  4 -5 (min)

                0 -1  0  0 -2  >=  1
                1  0 -3  0  4  >= -2
                0  3  0 -5  0  >=  3
                0  0  5  0 -6  >= -4
                2 -4  0  6  0  >=  5 
				
            <=>
            primal:
            max 1 -2  3 -4  5
                0  1  0  0  2  <= -1
               -1  0  3  0 -4  <=  2
                0 -3  0  5  0  <= -3
                0  0 -5  0  6  <=  4
               -2  4  0 -6  0  <= -5 
			
            dual:
            min-1  2 -3  4 -5
                0 -1  0  0 -2  >=  1 (*)
                1  0 -3  0  4  >= -2 	
                0  3  0 -5  0  >=  3
                0  0  5  0 -6  >= -4
                2 -4  0  6  0  >=  5
                */

            Model model = new Model();
            Solver solver = new Solver(model, solverType);

            Variable x1, x2, x3, x4, x5;

            Variable[] xArray = Variable.NewVariables(5);
            int i = 0;
            foreach (Variable x in xArray)
            {
                x.Name = "x" + (++i);
            }

            x1 = xArray[0];
            x2 = xArray[1];
            x3 = xArray[2];
            x4 = xArray[3];
            x5 = xArray[4];

            Expression expr = xArray.Sum();
            Expression expr2 = -1.0 * expr;
            Objective obj = model.Objective = (-1.0 * expr);

            RangeConstraint r1 = (RangeConstraint)model.Add(1 <= -1 * x2 + 0 * x3 - 2 * x5 <= Double.MaxValue);

            solver.Generate();

            model.Objective.SetCoefficient(x2, 2.0);
            model.Objective.SetCoefficient(x3, -3.0);
            model.Objective.SetCoefficient(x4, 4.0);
            model.Objective.SetCoefficient(x5, -5.0);

            model.ObjectiveSense = ObjectiveSense.Minimise;


            double[] coefs = { 1.0, 0.0, -3.0, 0.0, 4.0 };
            RangeConstraint r2 = (RangeConstraint)model.Add(-2.0 <= Expression.ScalarProduct(coefs, xArray) <= Double.MaxValue);

            System.Collections.Generic.LinkedList<double> morecoefs = new System.Collections.Generic.LinkedList<double>(new double[] { 0, 3, 0, -5, 0 });

            //Constraint r3 = model.Add(3 * x2 - 5 * x4 >= 3);
            Constraint r3 = model.Add(Expression.ScalarProduct(morecoefs, xArray) >= 3);
            Constraint r4 = model.Add(5 * x3 - 6 * x5 >= -4.0);
            RangeConstraint r5 = (RangeConstraint)model.Add(5.0 <= 2 * x1 - 4 * x2 + 6 * x4 <= Double.MaxValue);

            solver.Solve();


            Assert(!solver.IsProvenOptimal);
            Assert(solver.IsProvenPrimalInfeasible);
            Assert(!solver.IsProvenDualInfeasible);

            r1.Enabled = false;
            solver.Solve();


            Assert(!solver.IsProvenOptimal);
            Assert(!solver.IsProvenPrimalInfeasible);
            Assert(solver.IsProvenDualInfeasible);		// because of the first dual constraint -> because of the first primal variable.
        }

        /// <summary>
        /// Test default variable bounds: no given bound sets [0, inf)
        /// 
        /// </summary>
        /// <param name="model"></param>
        public void SonnetTest15()
        {
            Console.WriteLine("SonnetTest15");

            Model model = new Model();
            Solver solver = new Solver(model, solverType);

            Variable x1 = new Variable();
            Variable x2 = new Variable();

            model.Objective = (x1 - x2);
            model.ObjectiveSense = ObjectiveSense.Minimise;
            model.Add(x1 == -1);

            solver.Solve();


            Assert(!solver.IsProvenOptimal);
            Assert(solver.IsProvenPrimalInfeasible || solver.IsProvenDualInfeasible);
        }

        /// <summary>
        /// This tests the resolve: in a min. problem, increase the objective function coefs
        /// of variables that are _not_ in the initial solution shouldnt change in the resolve, and the resolve must be quick (pref. 0 iterations)
        /// </summary>
        /// <param name="model"></param>
        public void SonnetTest16()
        {
            Console.WriteLine("SonnetTest16");

            Model model = new Model();
            Solver solver = new Solver(model, solverType);

            Random rnd = new Random(1);

            const int nVars = 400;
            const int nCons = 400;
            const int nVarsPerCon = 5;

            Expression obj = new Expression();
            Variable[] xArray = new Variable[nVars];
            for (int i = 0; i < nVars; i++)
            {
                xArray[i] = new Variable("var" + i);
                obj.Add(1.0, xArray[i]);
            }
            model.Objective = Expression.Sum(xArray);
            model.ObjectiveSense = ObjectiveSense.Minimise;

            Constraint[] conArray = new Constraint[nCons];
            for (int j = 0; j < nCons; j++)
            {
                Expression expr = new Expression();
                for (int i = 0; i < nVarsPerCon; i++)
                {
                    int v = rnd.Next(nVars);
                    double coef = (double)rnd.Next(1, 40);
                    expr.Add(coef, xArray[v]);
                }
                if (j % 1000 == 0) Console.WriteLine("Constructed " + j + " constraints.");
                conArray[j] = expr >= 100.0;
                conArray[j].Name = "con" + j.ToString();
            }
            model.Add(conArray);
            solver.Generate();


            solver.Solve();


            Assert(solver.IsProvenOptimal);
            Assert(!solver.IsProvenPrimalInfeasible);
            Assert(!solver.IsProvenDualInfeasible);

            int firstIterationCount = solver.OsiSolver.getIterationCount();

            Objective objective = model.Objective;
            // in this min. problem, increasing the objective function value of a variable that was at its lower bound shouldnt change the solution
            foreach (Variable var in xArray)
            {
                double oldcoef = objective.GetCoefficient(var);
                if (var.Value == 0.0 && oldcoef > 0.0)
                {
                    objective.SetCoefficient(var, oldcoef * 1.1);
                    break;
                }
            }

            solver.Resolve();


            int secondIterationCount = solver.OsiSolver.getIterationCount();
            Assert(secondIterationCount < 5);	// allow for a few iteration, but basically, 0 should be enough
        }

        public void SonnetTest17()
        {
            Console.WriteLine("SonnetTest17");

            Model model = new Model();
            Solver solver = new Solver(model, solverType);

            Variable x0 = new Variable("x0", 0.0, 10.0);
            Variable x1 = new Variable("x1", 0.0, 10.0);

            Constraint con0 = x0 * 2 + x1 * 1 <= 10;
            Constraint con1 = x0 * 1 + x1 * 3 <= 15;

            model.Add(con0, "con0");

            solver.Generate();

            model.Add(con1, "con1");

            Objective obj = model.Objective = (x0 * 3 + x1 * 1);

            model.ObjectiveSense = ObjectiveSense.Maximise;

            solver.Solve();
            int iterationCount1 = solver.IterationCount;


            Assert(solver.IsProvenOptimal);
            Assert(!solver.IsProvenPrimalInfeasible);
            Assert(!solver.IsProvenDualInfeasible);

            Assert(MathExtension.CompareDouble(x0.Value, 5.0) == 0 &&
                MathExtension.CompareDouble(x1.Value, 0.0) == 0);

            Assert(MathExtension.CompareDouble(model.GetConstraint("con0").Value, 10.0) == 0 &&
                MathExtension.CompareDouble(model.GetConstraint("con1").Value, 5.0) == 0);

            Objective obj2 = new Objective(x0 + x1);
            model.Objective = obj2;

            Assert(!obj.AssignedTo(solver));

            solver.Resolve();
            int iterationCount2 = solver.IterationCount;


            Assert(solver.IsProvenOptimal);
            Assert(!solver.IsProvenPrimalInfeasible);
            Assert(!solver.IsProvenDualInfeasible);

            Assert(MathExtension.CompareDouble(x0.Value, 3.0) == 0 &&
                MathExtension.CompareDouble(x1.Value, 4.0) == 0);

            Assert(MathExtension.CompareDouble(model.GetConstraint("con0").Value, 10.0) == 0 &&
                MathExtension.CompareDouble(model.GetConstraint("con1").Value, 15.0) == 0);


            //model.SetObjectiveCoefficient(x0, 3);
            obj2.SetCoefficient(x0, 3);
            model.Objective.SetCoefficient(x1, 2);

            Assert(MathExtension.CompareDouble(obj2.GetCoefficient(x1), 2.0) == 0);
            Assert(MathExtension.CompareDouble(obj2.GetCoefficient(x0), 3.0) == 0);
            obj2.SetCoefficient(x1, 1);

            solver.Resolve();
            int iterationCount3 = solver.IterationCount;


            Assert(solver.IsProvenOptimal);
            Assert(!solver.IsProvenPrimalInfeasible);
            Assert(!solver.IsProvenDualInfeasible);

            Assert(MathExtension.CompareDouble(x0.Value, 5.0) == 0 &&
                MathExtension.CompareDouble(x1.Value, 0.0) == 0);

            Assert(MathExtension.CompareDouble(model.GetConstraint("con0").Value, 10.0) == 0 &&
                MathExtension.CompareDouble(model.GetConstraint("con1").Value, 5.0) == 0);

            model.Objective = obj;

            solver.Resolve();


            Assert(solver.IsProvenOptimal);
            Assert(!solver.IsProvenPrimalInfeasible);
            Assert(!solver.IsProvenDualInfeasible);

            Assert(MathExtension.CompareDouble(x0.Value, 5.0) == 0 &&
                MathExtension.CompareDouble(x1.Value, 0.0) == 0);

            Assert(MathExtension.CompareDouble(model.GetConstraint("con0").Value, 10.0) == 0 &&
                MathExtension.CompareDouble(model.GetConstraint("con1").Value, 5.0) == 0);

        }

        public void SonnetTest18()
        {
            Console.WriteLine("SonnetTest18");

            Model model = new Model();
            Solver solver = new Solver(model, solverType);

            Variable x1 = new Variable("x1");
            Variable x2 = new Variable("x2");
            Variable x3 = new Variable("x3");

            model.Add(x1 * 2 + x2 * 1 <= 10);
            model.Add(x1 * 1 + x2 * 3 <= 15);

            solver.Generate();

            model.Objective = (3.0 * x1 + 1.0 * x2 - x3);
            model.ObjectiveSense = ObjectiveSense.Maximise;

            solver.Solve();


            Assert(solver.IsProvenOptimal);
            Assert(!solver.IsProvenPrimalInfeasible);
            Assert(!solver.IsProvenDualInfeasible);

            Assert(MathExtension.CompareDouble(x1.Value, 5.0) == 0 &&
                MathExtension.CompareDouble(x2.Value, 0.0) == 0 &&
                MathExtension.CompareDouble(x3.Value, 0.0) == 0);
        }

        public void SonnetTest19()
        {
            Console.WriteLine("SonnetTest19");

            Model model = new Model();
            Solver solver = new Solver(model, solverType);

            Variable x1 = new Variable("x1");
            Variable x2 = new Variable("x2");
            Variable x3 = new Variable("x3");

            model.Add(x1 * 2 + x2 * 1 <= 10);
            model.Add(x1 * 1 + x2 * 3 <= 15);

            solver.Generate();

            model.Objective = (3.0 * x1 + 1.0 * x2 - x3);
            model.ObjectiveSense = (ObjectiveSense.Maximise);

            solver.Solve();


            Assert(solver.IsProvenOptimal);
            Assert(!solver.IsProvenPrimalInfeasible);
            Assert(!solver.IsProvenDualInfeasible);

            Assert(MathExtension.CompareDouble(x1.Value, 5.0) == 0 &&
                MathExtension.CompareDouble(x2.Value, 0.0) == 0 &&
                MathExtension.CompareDouble(x3.Value, 0.0) == 0);

            // TEST ALL HINTPARAMETERS and HINTSTRENGTHS
            foreach (OsiHintParam hintParam in Enum.GetValues(typeof(OsiHintParam)))
            {
                if (hintParam == OsiHintParam.OsiLastHintParam) continue;

                if (hintParam == OsiHintParam.OsiDoReducePrint) continue; // let's not mess with that

                bool yesNoTest = false;
                OsiHintStrength strengthTest = OsiHintStrength.OsiHintIgnore;
                solver.OsiSolver.getHintParam(hintParam, out yesNoTest, out strengthTest); // returns only if the given parameter and strength are valid values; otherwise exception

                for (int j = 0; j < (int)OsiHintStrength.OsiForceDo; j++) // SKIP ForceDo
                {
                    OsiHintStrength hintStrength = (OsiHintStrength)j;

                    // test get and set the hint param at given strength to TRUE
                    solver.OsiSolver.setHintParam(hintParam, true, hintStrength);
                    solver.OsiSolver.getHintParam(hintParam, out yesNoTest, out strengthTest);

                    Assert(yesNoTest == true);
                    Assert(strengthTest == hintStrength);

                    // test get and set the hint param at given strength to FALSE
                    solver.OsiSolver.setHintParam(hintParam, false, hintStrength);
                    solver.OsiSolver.getHintParam(hintParam, out yesNoTest, out strengthTest);

                    Assert(yesNoTest == false);
                    Assert(strengthTest == hintStrength);
                }

                solver.OsiSolver.setHintParam(hintParam, false, OsiHintStrength.OsiHintIgnore);
            }
        }

        public void SonnetTest0()
        {
            Console.WriteLine("SonnetTest0");

            Model model = new Model();
            Solver solver = new Solver(model, solverType);

            GetNumberOfVariablesOfVariableClass().SetValue(null, 0);
            GetNumberOfConstraintsOfConstraintClass().SetValue(null, 0);
            GetNumberOfObjectivesOfObjectiveClass().SetValue(null, 0);

            if (solverType != SolverType.CpxSolver) solver.OsiSolver.setIntParam(OsiIntParam.OsiNameDiscipline, 2);

            Variable x1 = new Variable();
            x1.Name = "x1";
            Variable x2 = new Variable("x2");
            Variable x3 = new Variable("x3", -1.0, 1.0, VariableType.Continuous);
            Variable x4 = new Variable("x4", -1.0, 1.0);
            Variable x5 = new Variable("x5", VariableType.Continuous);
            x5.Name = "x5";
            Variable x6 = new Variable(0.0, 2.0, VariableType.Continuous);
            x6.Name = "x6";
            Variable x7 = new Variable(VariableType.Integer);
            x7.Name = "x7";
            Variable x8 = new Variable(-1.0, 2.0);
            x8.Name = "x8";

            int i = 0;
            Expression[] exprs = new Expression[100];
            Expression expr1 = exprs[i++] = new Expression(x1);					// x1
            Expression expr2 = exprs[i++] = new Expression(x1 + 2.0 * x2);		// x1 + 2.0 x2
            Expression expr3 = exprs[i++] = new Expression(1.0);				// 1
            Expression expr4 = exprs[i++] = new Expression();					// 0
            Expression expr5 = exprs[i++] = new Expression(x1);					// x1
            Expression expr6 = exprs[i++] = new Expression(expr1.Add(5 * x2));	// x1 + 5 x2
            //expr1 = exprs[0] = new Expression(x1);
            Expression expr7 = exprs[i++] = expr1 + 5.0;						// x1 + 5 x2 + 5
            Expression expr8 = exprs[i++] = new Expression(x1);					// x1
            Expression expr9 = exprs[i++] = x2 + x3;	// returns new			// x2 + x3
            Expression expr10 = exprs[i++] = 1 + x4;							// x4 + 1
            Expression expr11 = exprs[i++] = x5 + 1;							// x5 + 1
            Expression expr12 = exprs[i++] = 1.0 * x6;							// x6
            Expression expr13 = exprs[i++] = x7 * 2.0;							// 2 x7
            Expression expr14 = exprs[i++] = x8 / 2.0;							// 0.5 x8
            Expression expr15 = exprs[i++] = new Expression(1.0);				// 1
            //Expression expr16 = exprs[i++] = new Expression(expr1.Add(1.0).Add(x1).Add(expr2).Add(3.5, x8));
            Expression expr16 = exprs[i++] = new Expression(expr1.Add(1.0).Add(x1).Add(expr2).Add(2.0 * 3.5, expr14));
            //  x1 + 5 x2 + x1 + x1 + 2.0 x2 + 3.5 x8 + 1
            //expr1 = exprs[0] = new Expression(x1);
            Expression expr17 = exprs[i++] = (new Expression()).Subtract(1.0).Subtract(x1).Subtract(expr2).Subtract(3.5, x8);
            //- x1 - x1 - 2.0 x2 - 3.5 x8 - 1
            Expression expr18 = exprs[i++] = (new Expression()).Divide(5.0).Multiply(3.2);
            //  0
            Expression expr19 = exprs[i++] = expr1 + x1;						// x1 + 5 x2 + x1 + x1 + 2.0 x2 + 3.5 x8 + x1 + 1
            Expression expr20 = exprs[i++] = x1 + expr1;						// x1 + x1 + 5 x2 + x1 + x1 + 2.0 x2 + 3.5 x8 + x1 + 1
            Expression expr21 = exprs[i++] = x1 - expr1;						// x1 - x1 - 5 x2 - x1 - x1 - 2.0 x2 - 3.5 x8 - x1 - 1
            Expression expr22 = exprs[i++] = x1 - x2;							// x1 - x2
            Expression expr23 = exprs[i++] = x1 - 1.0;							// x1 - 1
            Expression expr24 = exprs[i++] = 1.0 - x1;							// - x1 + 1
            Expression expr25 = exprs[i++] = 1.0 - expr1;						// - x1 - 5 x2 - x1 - x1 - 2.0 x2 - 3.5 x8 - x1 
            Expression expr26 = exprs[i++] = 1.0 + expr1;						// x1 + 5 x2 + x1 + x1 + 2.0 x2 + 3.5 x8 + x1 + 2
            Expression expr27 = exprs[i++] = expr1 / 2.0;						// 0.5 x1 + 2.5 x2 + 0.5 x1 + 0.5 x1 + 1.0 x2 + 1.75 x8 + 0.5
            Expression expr28 = exprs[i++] = x1 / 2.0;							// 0.5 x1

            List<Constraint> rawConstraints = new List<Constraint>();

            int n = i;
            for (i = 0; i < n; i++)
            {
                Constraint constraint = 1.0 * x1 <= exprs[i];
                constraint.Name = "Expr_con" + i;
                rawConstraints.Add(constraint);
            }

            model.Add(rawConstraints);

            i = -1;
            Constraint[] cons = new Constraint[100];
            cons[++i] = 1.0 == x1;
            cons[++i] = x1 == 1.0;
            cons[++i] = x2 == x1;
            cons[++i] = x1 == expr1;
            cons[++i] = expr1 + x1 == x1;
            cons[++i] = 1.0 == expr1;
            cons[++i] = expr1 == 1.0;
            cons[++i] = 1.0 >= x3;
            cons[++i] = 1.0 >= expr1;
            cons[++i] = x2 >= 1.0;
            cons[++i] = x2 >= x3;
            cons[++i] = x2 >= expr1;
            int m = i + 1;

            for (i = 0; i < m; i++)
            {
                model.Add(cons[i]);
            }

            string referenceUngeneratedModelToString =
                "x1 : Continuous : [0, Inf]\n" +
                "x2 : Continuous : [0, Inf]\n" +
                "x8 : Continuous : [-1, 2]\n" +
                "x3 : Continuous : [-1, 1]\n" +
                "x4 : Continuous : [-1, 1]\n" +
                "x5 : Continuous : [0, Inf]\n" +
                "x6 : Continuous : [0, 2]\n" +
                "x7 : Integer : [0, Inf]\n" +
                "Objective: Objective obj : 0\n" +
                "Constraints:\n" +
                "Expr_con0 : x1 <= x1 + 5 x2 + x1 + x1 + 2 x2 + 3.5 x8 + 1\n" +
                "Expr_con1 : x1 <= x1 + 2 x2\n" +
                "Expr_con2 : x1 <= 1\n" +
                "Expr_con3 : x1 <= 0\n" +
                "Expr_con4 : x1 <= x1\n" +
                "Expr_con5 : x1 <= x1 + 5 x2\n" +
                "Expr_con6 : x1 <= x1 + 5 x2 + 5\n" +
                "Expr_con7 : x1 <= x1\n" +
                "Expr_con8 : x1 <= x2 + x3\n" +
                "Expr_con9 : x1 <= x4 + 1\n" +
                "Expr_con10 : x1 <= x5 + 1\n" +
                "Expr_con11 : x1 <= x6\n" +
                "Expr_con12 : x1 <= 2 x7\n" +
                "Expr_con13 : x1 <= 0.5 x8\n" +
                "Expr_con14 : x1 <= 1\n" +
                "Expr_con15 : x1 <= x1 + 5 x2 + x1 + x1 + 2 x2 + 3.5 x8 + 1\n" +
                "Expr_con16 : x1 <= - x1 - x1 - 2 x2 - 3.5 x8 - 1\n" +
                "Expr_con17 : x1 <= 0\n" +
                "Expr_con18 : x1 <= x1 + 5 x2 + x1 + x1 + 2 x2 + 3.5 x8 + x1 + 1\n" +
                "Expr_con19 : x1 <= x1 + x1 + 5 x2 + x1 + x1 + 2 x2 + 3.5 x8 + 1\n" +
                "Expr_con20 : x1 <= x1 - x1 - 5 x2 - x1 - x1 - 2 x2 - 3.5 x8 - 1\n" +
                "Expr_con21 : x1 <= x1 - x2\n" +
                "Expr_con22 : x1 <= x1 - 1\n" +
                "Expr_con23 : x1 <= - x1 + 1\n" +
                "Expr_con24 : x1 <= - x1 - 5 x2 - x1 - x1 - 2 x2 - 3.5 x8\n" +
                "Expr_con25 : x1 <= x1 + 5 x2 + x1 + x1 + 2 x2 + 3.5 x8 + 2\n" +
                "Expr_con26 : x1 <= 0.5 x1 + 2.5 x2 + 0.5 x1 + 0.5 x1 + x2 + 1.75 x8 + 0.5\n" +
                "Expr_con27 : x1 <= 0.5 x1\n" +
                "Con[28] : 1 == x1\n" +
                "Con[29] : x1 == 1\n" +
                "Con[30] : x2 == x1\n" +
                "Con[31] : x1 == x1 + 5 x2 + x1 + x1 + 2 x2 + 3.5 x8 + 1\n" +
                "Con[32] : x1 + 5 x2 + x1 + x1 + 2 x2 + 3.5 x8 + x1 + 1 == x1\n" +
                "Con[33] : 1 == x1 + 5 x2 + x1 + x1 + 2 x2 + 3.5 x8 + 1\n" +
                "Con[34] : x1 + 5 x2 + x1 + x1 + 2 x2 + 3.5 x8 + 1 == 1\n" +
                "Con[35] : 1 >= x3\n" +
                "Con[36] : 1 >= x1 + 5 x2 + x1 + x1 + 2 x2 + 3.5 x8 + 1\n" +
                "Con[37] : x2 >= 1\n" +
                "Con[38] : x2 >= x3\n" +
                "Con[39] : x2 >= x1 + 5 x2 + x1 + x1 + 2 x2 + 3.5 x8 + 1\n";

            string modelUngeneratedString = model.ToString();
            Assert(SonnetTest.EqualsString(modelUngeneratedString, referenceUngeneratedModelToString));

            solver.Generate();

            int istart = i + 1;
            cons[++i] = expr1 >= 1.0;
            cons[++i] = expr1 >= x1;
            cons[++i] = expr1 >= expr2;
            cons[++i] = 1.0 <= x3;
            cons[++i] = 1.0 <= expr1;
            cons[++i] = x2 <= 1.0;
            cons[++i] = x2 <= x3;
            cons[++i] = x2 <= expr1;
            cons[++i] = expr1 <= 1.0;
            cons[++i] = expr1 <= x1;
            cons[++i] = expr1 <= expr2;
            cons[++i] = new Constraint(expr1, ConstraintType.EQ, expr2);
            cons[++i] = new Constraint("mycon", expr3, ConstraintType.EQ, expr4);
            cons[++i] = new Constraint(cons[i - 1]);
            cons[++i] = new Constraint("othercon", cons[i - 1]);
            cons[++i] = new Expression() >= new Expression();
            cons[++i] = 1.2 * (new Variable("xNEW")) >= new Expression();
            m = i + 1;

            for (i = istart; i < m; i++)
            {
                model.Add(cons[i]);
            }

            solver.Generate();

            string referenceGeneratedModel;

            referenceGeneratedModel =
                    "x1 : Continuous : [0, Inf]\n" +
                    "x2 : Continuous : [0, Inf]\n" +
                    "x8 : Continuous : [-1, 2]\n" +
                    "x3 : Continuous : [-1, 1]\n" +
                    "x7 : Integer : [0, Inf]\n" +
                    "x6 : Continuous : [0, 2]\n" +
                    "x5 : Continuous : [0, Inf]\n" +
                    "x4 : Continuous : [-1, 1]\n" +
                    "xNEW : Continuous : [0, Inf]\n" +
                    "Objective: Objective obj : 0\n" +
                    "Constraints:\n" +
                    "Expr_con0 : - 2 x1 - 7 x2 - 3.5 x8 <= 1\n" +
                    "Con[39] : - 3 x1 - 6 x2 - 3.5 x8 >= 1\n" +
                    "Con[38] : x2 - x3 >= 0\n" +
                    "Con[37] : x2 >= 1\n" +
                    "Con[36] : - 3 x1 - 7 x2 - 3.5 x8 >= 0\n" +
                    "Con[35] : - x3 >= -1\n" +
                    "Con[34] : 3 x1 + 7 x2 + 3.5 x8 == 0\n" +
                    "Con[33] : - 3 x1 - 7 x2 - 3.5 x8 == 0\n" +
                    "Con[32] : 3 x1 + 7 x2 + 3.5 x8 == -1\n" +
                    "Con[31] : - 2 x1 - 7 x2 - 3.5 x8 == 1\n" +
                    "Con[30] : - x1 + x2 == 0\n" +
                    "Con[29] : x1 == 1\n" +
                    "Con[28] : - x1 == -1\n" +
                    "Expr_con27 : 0.5 x1 <= 0\n" +
                    "Expr_con26 : - 0.5 x1 - 3.5 x2 - 1.75 x8 <= 0.5\n" +
                    "Expr_con25 : - 2 x1 - 7 x2 - 3.5 x8 <= 2\n" +
                    "Expr_con24 : 4 x1 + 7 x2 + 3.5 x8 <= 0\n" +
                    "Expr_con23 : 2 x1 <= 1\n" +
                    "Expr_con22 : 0 <= -1\n" +
                    "Expr_con21 : x2 <= 0\n" +
                    "Expr_con20 : 3 x1 + 7 x2 + 3.5 x8 <= -1\n" +
                    "Expr_con19 : - 3 x1 - 7 x2 - 3.5 x8 <= 1\n" +
                    "Expr_con18 : - 3 x1 - 7 x2 - 3.5 x8 <= 1\n" +
                    "Expr_con17 : x1 <= 0\n" +
                    "Expr_con16 : 3 x1 + 2 x2 + 3.5 x8 <= -1\n" +
                    "Expr_con15 : - 2 x1 - 7 x2 - 3.5 x8 <= 1\n" +
                    "Expr_con14 : x1 <= 1\n" +
                    "Expr_con13 : x1 - 0.5 x8 <= 0\n" +
                    "Expr_con12 : x1 - 2 x7 <= 0\n" +
                    "Expr_con11 : x1 - x6 <= 0\n" +
                    "Expr_con10 : x1 - x5 <= 1\n" +
                    "Expr_con9 : x1 - x4 <= 1\n" +
                    "Expr_con8 : x1 - x2 - x3 <= 0\n" +
                    "Expr_con7 : 0 <= 0\n" +
                    "Expr_con6 : - 5 x2 <= 5\n" +
                    "Expr_con5 : - 5 x2 <= 0\n" +
                    "Expr_con4 : 0 <= 0\n" +
                    "Expr_con3 : x1 <= 0\n" +
                    "Expr_con2 : x1 <= 1\n" +
                    "Expr_con1 : - 2 x2 <= 0\n" +
                    "Con[40] : 3 x1 + 7 x2 + 3.5 x8 >= 0\n" +
                    "Con[56] : 1.2 xNEW >= 0\n" +
                    "Con[55] : 0 >= 0\n" +
                    "othercon : 0 == -1\n" +
                    "Con[53] : 0 == -1\n" +
                    "mycon : 0 == -1\n" +
                    "Con[51] : 2 x1 + 5 x2 + 3.5 x8 == -1\n" +
                    "Con[50] : 2 x1 + 5 x2 + 3.5 x8 <= -1\n" +
                    "Con[49] : 2 x1 + 7 x2 + 3.5 x8 <= -1\n" +
                    "Con[48] : 3 x1 + 7 x2 + 3.5 x8 <= 0\n" +
                    "Con[47] : - 3 x1 - 6 x2 - 3.5 x8 <= 1\n" +
                    "Con[46] : x2 - x3 <= 0\n" +
                    "Con[45] : x2 <= 1\n" +
                    "Con[44] : - 3 x1 - 7 x2 - 3.5 x8 <= 0\n" +
                    "Con[43] : - x3 <= -1\n" +
                    "Con[42] : 2 x1 + 5 x2 + 3.5 x8 >= -1\n" +
                    "Con[41] : 2 x1 + 7 x2 + 3.5 x8 >= -1\n";

            string modelString = solver.ToString();
            Assert(SonnetTest.EqualsString(modelString, referenceGeneratedModel));

            solver.ExportModel("test.mps");
            System.IO.StreamReader test = new System.IO.StreamReader("test.mps");
            string testMps = test.ReadToEnd();

            string referenceTestMps;

            referenceTestMps =
                "NAME BLANK FREE\n" +
                "ROWS\n" +
                " N OBJROW\n" +
                " L Expr_con0\n" +
                " G Con[39]\n" +
                " G Con[38]\n" +
                " G Con[37]\n" +
                " G Con[36]\n" +
                " G Con[35]\n" +
                " E Con[34]\n" +
                " E Con[33]\n" +
                " E Con[32]\n" +
                " E Con[31]\n" +
                " E Con[30]\n" +
                " E Con[29]\n" +
                " E Con[28]\n" +
                " L Expr_con27\n" +
                " L Expr_con26\n" +
                " L Expr_con25\n" +
                " L Expr_con24\n" +
                " L Expr_con23\n" +
                " L Expr_con22\n" +
                " L Expr_con21\n" +
                " L Expr_con20\n" +
                " L Expr_con19\n" +
                " L Expr_con18\n" +
                " L Expr_con17\n" +
                " L Expr_con16\n" +
                " L Expr_con15\n" +
                " L Expr_con14\n" +
                " L Expr_con13\n" +
                " L Expr_con12\n" +
                " L Expr_con11\n" +
                " L Expr_con10\n" +
                " L Expr_con9\n" +
                " L Expr_con8\n" +
                " L Expr_con7\n" +
                " L Expr_con6\n" +
                " L Expr_con5\n" +
                " L Expr_con4\n" +
                " L Expr_con3\n" +
                " L Expr_con2\n" +
                " L Expr_con1\n" +
                " G Con[40]\n" +
                " G Con[56]\n" +
                " G Con[55]\n" +
                " E othercon\n" +
                " E Con[53]\n" +
                " E mycon\n" +
                " E Con[51]\n" +
                " L Con[50]\n" +
                " L Con[49]\n" +
                " L Con[48]\n" +
                " L Con[47]\n" +
                " L Con[46]\n" +
                " L Con[45]\n" +
                " L Con[44]\n" +
                " L Con[43]\n" +
                " G Con[42]\n" +
                " G Con[41]\n" +
                "COLUMNS\n" +
                " x1 Expr_con0 -2. Con[39] -3. \n" +
                " x1 Con[36] -3. Con[34] 3. \n" +
                " x1 Con[33] -3. Con[32] 3. \n" +
                " x1 Con[31] -2. Con[30] -1. \n" +
                " x1 Con[29] 1. Con[28] -1. \n" +
                " x1 Expr_con27 0.5 Expr_con26 -0.5 \n" +
                " x1 Expr_con25 -2. Expr_con24 4. \n" +
                " x1 Expr_con23 2. Expr_con20 3. \n" +
                " x1 Expr_con19 -3. Expr_con18 -3. \n" +
                " x1 Expr_con17 1. Expr_con16 3. \n" +
                " x1 Expr_con15 -2. Expr_con14 1. \n" +
                " x1 Expr_con13 1. Expr_con12 1. \n" +
                " x1 Expr_con11 1. Expr_con10 1. \n" +
                " x1 Expr_con9 1. Expr_con8 1. \n" +
                " x1 Expr_con3 1. Expr_con2 1. \n" +
                " x1 Con[40] 3. Con[51] 2. \n" +
                " x1 Con[50] 2. Con[49] 2. \n" +
                " x1 Con[48] 3. Con[47] -3. \n" +
                " x1 Con[44] -3. Con[42] 2. \n" +
                " x1 Con[41] 2. \n" +
                " x2 Expr_con0 -7. Con[39] -6. \n" +
                " x2 Con[38] 1. Con[37] 1. \n" +
                " x2 Con[36] -7. Con[34] 7. \n" +
                " x2 Con[33] -7. Con[32] 7. \n" +
                " x2 Con[31] -7. Con[30] 1. \n" +
                " x2 Expr_con26 -3.5 Expr_con25 -7. \n" +
                " x2 Expr_con24 7. Expr_con21 1. \n" +
                " x2 Expr_con20 7. Expr_con19 -7. \n" +
                " x2 Expr_con18 -7. Expr_con16 2. \n" +
                " x2 Expr_con15 -7. Expr_con8 -1. \n" +
                " x2 Expr_con6 -5. Expr_con5 -5. \n" +
                " x2 Expr_con1 -2. Con[40] 7. \n" +
                " x2 Con[51] 5. Con[50] 5. \n" +
                " x2 Con[49] 7. Con[48] 7. \n" +
                " x2 Con[47] -6. Con[46] 1. \n" +
                " x2 Con[45] 1. Con[44] -7. \n" +
                " x2 Con[42] 5. Con[41] 7. \n" +
                " x8 Expr_con0 -3.5 Con[39] -3.5 \n" +
                " x8 Con[36] -3.5 Con[34] 3.5 \n" +
                " x8 Con[33] -3.5 Con[32] 3.5 \n" +
                " x8 Con[31] -3.5 Expr_con26 -1.75 \n" +
                " x8 Expr_con25 -3.5 Expr_con24 3.5 \n" +
                " x8 Expr_con20 3.5 Expr_con19 -3.5 \n" +
                " x8 Expr_con18 -3.5 Expr_con16 3.5 \n" +
                " x8 Expr_con15 -3.5 Expr_con13 -0.5 \n" +
                " x8 Con[40] 3.5 Con[51] 3.5 \n" +
                " x8 Con[50] 3.5 Con[49] 3.5 \n" +
                " x8 Con[48] 3.5 Con[47] -3.5 \n" +
                " x8 Con[44] -3.5 Con[42] 3.5 \n" +
                " x8 Con[41] 3.5 \n" +
                " x3 Con[38] -1. Con[35] -1. \n" +
                " x3 Expr_con8 -1. Con[46] -1. \n" +
                " x3 Con[43] -1. \n" +
                " x7 Expr_con12 -2. \n" +
                " x6 Expr_con11 -1. \n" +
                " x5 Expr_con10 -1. \n" +
                " x4 Expr_con9 -1. \n" +
                " xNEW Con[56] 1.2 \n" +
                "RHS\n" +
                " RHS Expr_con0 1. Con[39] 1. \n" +
                " RHS Con[37] 1. Con[35] -1. \n" +
                " RHS Con[32] -1. Con[31] 1. \n" +
                " RHS Con[29] 1. Con[28] -1. \n" +
                " RHS Expr_con26 0.5 Expr_con25 2. \n" +
                " RHS Expr_con23 1. Expr_con22 -1. \n" +
                " RHS Expr_con20 -1. Expr_con19 1. \n" +
                " RHS Expr_con18 1. Expr_con16 -1. \n" +
                " RHS Expr_con15 1. Expr_con14 1. \n" +
                " RHS Expr_con10 1. Expr_con9 1. \n" +
                " RHS Expr_con6 5. Expr_con2 1. \n" +
                " RHS othercon -1. Con[53] -1. \n" +
                " RHS mycon -1. Con[51] -1. \n" +
                " RHS Con[50] -1. Con[49] -1. \n" +
                " RHS Con[47] 1. Con[45] 1. \n" +
                " RHS Con[43] -1. Con[42] -1. \n" +
                " RHS Con[41] -1. \n" +
                "BOUNDS\n" +
                " LO BOUND x8 -1. \n" +
                " UP BOUND x8 2. \n" +
                " LO BOUND x3 -1. \n" +
                " UP BOUND x3 1. \n" +
                " UI BOUND x7 1e+30\n" +
                " UP BOUND x6 2. \n" +
                " LO BOUND x4 -1. \n" +
                " UP BOUND x4 1. \n" +
                "ENDATA\n";

            testMps = testMps.Replace("\r", "");
            while (testMps.IndexOf("  ") >= 0)
            {
                testMps = testMps.Replace("  ", " ");
            }

            if (solverType != SolverType.CpxSolver) Assert(SonnetTest.EqualsString(testMps, referenceTestMps));
        }

        public void SonnetTest0b()
        {
            Console.WriteLine("SonnetTest0b");

            Model model = new Model();
            Solver solver = new Solver(model, solverType);

            Assert(model.ImportModel("test.mps"));
            solver.Generate();

            Variable x7 = solver.GetVariable("x7");
            x7.Upper = model.Infinity;


            string referenceGeneratedModel;

                referenceGeneratedModel =
                    "x1 : Continuous : [0, Inf]\n" +
                    "x2 : Continuous : [0, Inf]\n" +
                    "x8 : Continuous : [-1, 2]\n" +
                    "x3 : Continuous : [-1, 1]\n" +
                    "xNEW : Continuous : [0, Inf]\n" +
                    "x4 : Continuous : [-1, 1]\n" +
                    "x5 : Continuous : [0, Inf]\n" +
                    "x6 : Continuous : [0, 2]\n" +
                    "x7 : Integer : [0, Inf]\n" +
                    "Objective: Objective OBJROW : 0\n" +
                    "Constraints:\n" +
                    "Expr_con0 : - 2 x1 - 7 x2 - 3.5 x8 <= 1\n" +
                    "Con[41] : 2 x1 + 7 x2 + 3.5 x8 >= -1\n" +
                    "Con[42] : 2 x1 + 5 x2 + 3.5 x8 >= -1\n" +
                    "Con[43] : - x3 <= -1\n" +
                    "Con[44] : - 3 x1 - 7 x2 - 3.5 x8 <= 0\n" +
                    "Con[45] : x2 <= 1\n" +
                    "Con[46] : x2 - x3 <= 0\n" +
                    "Con[47] : - 3 x1 - 6 x2 - 3.5 x8 <= 1\n" +
                    "Con[48] : 3 x1 + 7 x2 + 3.5 x8 <= 0\n" +
                    "Con[49] : 2 x1 + 7 x2 + 3.5 x8 <= -1\n" +
                    "Con[50] : 2 x1 + 5 x2 + 3.5 x8 <= -1\n" +
                    "Con[51] : 2 x1 + 5 x2 + 3.5 x8 == -1\n" +
                    "mycon : 0 == -1\n" +
                    "Con[53] : 0 == -1\n" +
                    "othercon : 0 == -1\n" +
                    "Con[55] : 0 >= 0\n" +
                    "Con[56] : 1.2 xNEW >= 0\n" +
                    "Con[40] : 3 x1 + 7 x2 + 3.5 x8 >= 0\n" +
                    "Expr_con1 : - 2 x2 <= 0\n" +
                    "Expr_con2 : x1 <= 1\n" +
                    "Expr_con3 : x1 <= 0\n" +
                    "Expr_con4 : 0 <= 0\n" +
                    "Expr_con5 : - 5 x2 <= 0\n" +
                    "Expr_con6 : - 5 x2 <= 5\n" +
                    "Expr_con7 : 0 <= 0\n" +
                    "Expr_con8 : x1 - x2 - x3 <= 0\n" +
                    "Expr_con9 : x1 - x4 <= 1\n" +
                    "Expr_con10 : x1 - x5 <= 1\n" +
                    "Expr_con11 : x1 - x6 <= 0\n" +
                    "Expr_con12 : x1 - 2 x7 <= 0\n" +
                    "Expr_con13 : x1 - 0.5 x8 <= 0\n" +
                    "Expr_con14 : x1 <= 1\n" +
                    "Expr_con15 : - 2 x1 - 7 x2 - 3.5 x8 <= 1\n" +
                    "Expr_con16 : 3 x1 + 2 x2 + 3.5 x8 <= -1\n" +
                    "Expr_con17 : x1 <= 0\n" +
                    "Expr_con18 : - 3 x1 - 7 x2 - 3.5 x8 <= 1\n" +
                    "Expr_con19 : - 3 x1 - 7 x2 - 3.5 x8 <= 1\n" +
                    "Expr_con20 : 3 x1 + 7 x2 + 3.5 x8 <= -1\n" +
                    "Expr_con21 : x2 <= 0\n" +
                    "Expr_con22 : 0 <= -1\n" +
                    "Expr_con23 : 2 x1 <= 1\n" +
                    "Expr_con24 : 4 x1 + 7 x2 + 3.5 x8 <= 0\n" +
                    "Expr_con25 : - 2 x1 - 7 x2 - 3.5 x8 <= 2\n" +
                    "Expr_con26 : - 0.5 x1 - 3.5 x2 - 1.75 x8 <= 0.5\n" +
                    "Expr_con27 : 0.5 x1 <= 0\n" +
                    "Con[28] : - x1 == -1\n" +
                    "Con[29] : x1 == 1\n" +
                    "Con[30] : - x1 + x2 == 0\n" +
                    "Con[31] : - 2 x1 - 7 x2 - 3.5 x8 == 1\n" +
                    "Con[32] : 3 x1 + 7 x2 + 3.5 x8 == -1\n" +
                    "Con[33] : - 3 x1 - 7 x2 - 3.5 x8 == 0\n" +
                    "Con[34] : 3 x1 + 7 x2 + 3.5 x8 == 0\n" +
                    "Con[35] : - x3 >= -1\n" +
                    "Con[36] : - 3 x1 - 7 x2 - 3.5 x8 >= 0\n" +
                    "Con[37] : x2 >= 1\n" +
                    "Con[38] : x2 - x3 >= 0\n" +
                    "Con[39] : - 3 x1 - 6 x2 - 3.5 x8 >= 1\n";

            string modelString = solver.ToString();

            if (solverType != SolverType.CpxSolver) Assert(SonnetTest.EqualsString(modelString, referenceGeneratedModel));
        }

        public void SonnetTest20()
        {
            Console.WriteLine("SonnetTest20");


            /*
            With this matrix we have a primal/dual infeas problem. Leaving the first
            row makes it primal feas, leaving the first col makes it dual feas. 
            (JWG but leaving the first col out of the primal representation will
            make the primal unfeasible..)
            All vars are >= 0

            obj:-1  2 -3  4 -5 (min)

                0 -1  0  0 -2  >=  1
                1  0 -3  0  4  >= -2
                0  3  0 -5  0  >=  3
                0  0  5  0 -6  >= -4
                2 -4  0  6  0  >=  5 
				
            <=>
            primal:
            max 1 -2  3 -4  5
                0  1  0  0  2  <= -1
               -1  0  3  0 -4  <=  2
                0 -3  0  5  0  <= -3
                0  0 -5  0  6  <=  4
               -2  4  0 -6  0  <= -5 
			
            dual:
            min-1  2 -3  4 -5
                0 -1  0  0 -2  >=  1 (*)
                1  0 -3  0  4  >= -2 	
                0  3  0 -5  0  >=  3
                0  0  5  0 -6  >= -4
                2 -4  0  6  0  >=  5
                */

            Model model = new Model();
            Solver solver = new Solver(model, solverType);

            Variable x1, x2, x3, x4, x5;

            List<Variable> xArray = new List<Variable>(5);
            for (int i = 0; i < 5; )
            {
                Variable x = new Variable();
                xArray.Add(x); // adds at the end!
                x.Name = "x" + (++i);
            }

            x1 = (Variable)xArray[0];
            x2 = (Variable)xArray[1];
            x3 = (Variable)xArray[2];
            x4 = (Variable)xArray[3];
            x5 = (Variable)xArray[4];

            Expression expr = Expression.Sum(xArray);
            Expression expr2 = -1.0 * expr;
            Objective obj = model.Objective = (-1.0 * expr);

            RangeConstraint r1 = (RangeConstraint)model.Add(1 <= -1 * x2 + 0 * x3 - 2 * x5 <= Double.MaxValue);

            solver.Generate();

            model.Objective.SetCoefficient(x2, 2.0);
            model.Objective.SetCoefficient(x3, -3.0);
            model.Objective.SetCoefficient(x4, 4.0);
            model.Objective.SetCoefficient(x5, -5.0);

            model.ObjectiveSense = ObjectiveSense.Minimise;

            double[] coefs = { 1.0, 0.0, -3.0, 0.0, 4.0 };
            RangeConstraint r2 = (RangeConstraint)model.Add(-2.0 <= xArray.ScalarProduct(coefs) <= Double.MaxValue);

            Constraint r3 = model.Add(3 * x2 - 5 * x4 >= 3);
            Constraint r4 = model.Add(5 * x3 - 6 * x5 >= -4.0);
            RangeConstraint r5 = (RangeConstraint)model.Add(5.0 <= 2 * x1 - 4 * x2 + 6 * x4 <= Double.MaxValue);

            solver.Solve();


            Assert(!solver.IsProvenOptimal);
            Assert(solver.IsProvenPrimalInfeasible);
            Assert(!solver.IsProvenDualInfeasible);

            r1.Enabled = false;
            solver.Solve();


            Assert(!solver.IsProvenOptimal);
            Assert(!solver.IsProvenPrimalInfeasible);
            Assert(solver.IsProvenDualInfeasible);		// because of the first dual constraint -> because of the first primal variable.
        }

        /// <summary>
        /// Test the Model::Contains(constraint)
        /// </summary>
        /// <param name="model"></param>
        public void SonnetTest21()
        {
            Console.WriteLine("SonnetTest21 - test Model::Contains(constraint)");

            Model model = new Model();
            Solver solver = new Solver(model, solverType);

            Variable x0 = new Variable("x0", 4, model.Infinity);
            Variable x1 = new Variable("x1", 3, model.Infinity);

            Objective obj = model.Objective = (3 * x0 + 1 * x1);

            Constraint r0 = x0 * 2 + x1 * 1 <= 10;
            Constraint r1 = x0 * 1 + x1 * 3 <= 15;
            Constraint r2 = x0 * 1 + x1 * 1 <= 10;

            model.Add(r0);

            model.ObjectiveSense = (ObjectiveSense.Maximise);

            Assert(model.Contains(r0));
            Assert(!model.Contains(r1));

            model.Add(r1);
            Assert(!solver.IsRegistered(r1));
            Assert(model.Contains(r1));

            solver.Solve();

            model.Add(r2);

            Assert(model.Contains(r0));
            Assert(solver.Contains(r0));
            Assert(solver.IsRegistered(r0));
            Assert(solver.IsRegistered(r1));
            Assert(!solver.IsRegistered(r2));

            Assert(model.Contains(r2));


        }

        /// <summary>
        /// Test the Model::Contains(constraint)
        /// </summary>
        /// <param name="model"></param>
        public void SonnetTest22()
        {
            Console.WriteLine("SonnetTest22");

            Model model = new Model();
            Solver solver;
            int value;

            solver = new Solver(model, SolverType.ClpSolver);
            solver.OsiSolver.setIntParam(OsiIntParam.OsiMaxNumIteration, 1234);
            solver.OsiSolver.getIntParam(OsiIntParam.OsiMaxNumIteration, out value);
            Assert(value == 1234);

            solver = new Solver(model, SolverType.CbcSolver);// new: we DONT copy parameters
            solver.OsiSolver.setIntParam(OsiIntParam.OsiMaxNumIteration, 1234);
            solver.OsiSolver.getIntParam(OsiIntParam.OsiMaxNumIteration, out value);
            Assert(value == 1234);

            solver = new Solver(model, SolverType.ClpSolver); // new: we DONT copy parameters
            solver.OsiSolver.setIntParam(OsiIntParam.OsiMaxNumIteration, 1234);
            solver.OsiSolver.getIntParam(OsiIntParam.OsiMaxNumIteration, out value);
            Assert(value == 1234);
        }

        public void SonnetTest23()
        {
            Console.WriteLine("SonnetTest23");

            Model model = new Model();
            Solver solver = new Solver(model, solverType);

            Variable x0 = new Variable();
            Variable x1 = new Variable();
            model.Add(18 * x0 >= 100);
            model.Objective = (x0 + x1);
            model.ObjectiveSense = ObjectiveSense.Minimise;

            //solver.OsiSolver.setHintParam(OsiHintParam.OsiDoPresolveInResolve, false, OsiHintStrength.OsiHintDo);
            solver.OsiSolver.setHintParam(OsiHintParam.OsiDoPresolveInInitial, true, OsiHintStrength.OsiHintDo);
            solver.OsiSolver.setHintParam(OsiHintParam.OsiDoDualInInitial, false, OsiHintStrength.OsiHintDo);
            //solver.OsiSolver.setHintParam(OsiHintParam.OsiDoDualInResolve, false, OsiHintStrength.OsiHintDo);

            solver.Generate();
            solver.Resolve(); // !

            Assert(MathExtension.CompareDouble(model.Objective.Value, 5.55555555) == 0);
            Assert(MathExtension.CompareDouble(model.Objective.Level(), 5.55555555) == 0);
            Assert(solver.IsProvenOptimal);
            Assert(!solver.IsProvenPrimalInfeasible);
            Assert(!solver.IsProvenDualInfeasible);
        }

        public void SonnetTest24()
        {
            Console.WriteLine("SonnetTest24");

            Model model = new Model();
            Solver solver = new Solver(model, solverType);

            //string bin = FindBinParent(Environment.CurrentDirectory);
            //string filePath = (bin != null && bin.Length > 0) ? bin + "\\..\\" : "";
            Assert(model.ImportModel("expect-feasible.mps")); // added file to project, "Copy Always"

            solver.Generate();
            solver.Resolve();

            Assert(MathExtension.CompareDouble(model.Objective.Value, 5.55555555) == 0);
            Assert(MathExtension.CompareDouble(model.Objective.Level(), 5.55555555) == 0);
            Assert(solver.IsProvenOptimal);
            Assert(!solver.IsProvenPrimalInfeasible);
            Assert(!solver.IsProvenDualInfeasible);
        }

        public void SonnetTest25()
        {
            Console.WriteLine("SonnetTest25");

            Model model = new Model();
            Solver solver = new Solver(model, solverType);

            Variable x0 = new Variable();
            x0.Type = VariableType.Integer;
            Variable x1 = new Variable();
            model.Add(x0 + 2 * x1 <= 3.9);
            model.Objective = (x0 + x1);
            model.ObjectiveSense = ObjectiveSense.Maximise;

            solver.Generate();
            int numElements = solver.NumberOfElements;
            int numInts = solver.NumberOfIntegerVariables;
            solver.Solve();

            Assert(solver.IsProvenOptimal);
            Assert(!solver.IsProvenPrimalInfeasible);
            Assert(!solver.IsProvenDualInfeasible);
            Assert(numElements == solver.NumberOfElements);
            Assert(numInts == solver.NumberOfIntegerVariables);

            // with resetaftermipsolve the objectivevalue() doesnt work.
            //Assert(MathExtension.CompareDouble(model.Objective.Value, model.ObjectiveValue()) == 0);
            Assert(MathExtension.CompareDouble(model.Objective.Value, 3.45) == 0);
            Assert(MathExtension.CompareDouble(x0.Value, 3.0) == 0);
            Assert(MathExtension.CompareDouble(x1.Value, 0.45) == 0);

            model.Add(x0 <= 2.0);
            solver.Solve();

            Assert(solver.IsProvenOptimal);
            Assert(!solver.IsProvenPrimalInfeasible);
            Assert(!solver.IsProvenDualInfeasible);

            Assert(MathExtension.CompareDouble(model.Objective.Value, 2.95) == 0);
            Assert(MathExtension.CompareDouble(x0.Value, 2.0) == 0);
            Assert(MathExtension.CompareDouble(x1.Value, 0.95) == 0);
        }

        public void SonnetTest26()
        {
            Console.WriteLine("SonnetTest26");

            Model model = new Model();
            Solver solver = new Solver(model, solverType);

            Variable x0 = new Variable();
            x0.Type = VariableType.Integer;
            Variable x1 = new Variable();
            model.Add(x0 + 2 * x1 <= 3.9);
            model.Objective = (x0 + x1);
            model.ObjectiveSense = (ObjectiveSense.Maximise);

            // FOR CLP, we have to supply upper bounds on the integer variables!
            // note: you dont really want to use CLP's B&B anyway..
            //x0.Upper = 1000.0; // should be fixed now.

            solver.Solve();

            Assert(solver.IsProvenOptimal);
            Assert(!solver.IsProvenPrimalInfeasible);
            Assert(!solver.IsProvenDualInfeasible);

            Assert(MathExtension.CompareDouble(model.Objective.Value, 3.45) == 0);
            Assert(MathExtension.CompareDouble(x0.Value, 3.0) == 0);
            Assert(MathExtension.CompareDouble(x1.Value, 0.45) == 0);
        }

        public void SonnetTest26b()
        {
            Console.WriteLine("SonnetTest26b");
            if (solverType != SolverType.CbcSolver) return;

            Model model = new Model();
            Solver solver = new Solver(model, solverType);

            Assert(model.ImportModel("egout.mps"));
            solver.Generate();
            int numElements = solver.NumberOfElements;
            int numInts = solver.NumberOfIntegerVariables;

            solver.Solve();

            // this fails.. why does the CBC Reset/save after mip not work??
            solver.Solve();

            Assert(solver.IsProvenOptimal);
            Assert(!solver.IsProvenPrimalInfeasible);
            Assert(!solver.IsProvenDualInfeasible);
            Assert(MathExtension.CompareDouble(model.Objective.Value, 568.1007) == 0);
            Assert(numElements == solver.NumberOfElements);
            Assert(numInts == solver.NumberOfIntegerVariables);
        }

        public void SonnetTest27()
        {
            Console.WriteLine("SonnetTest27");

            Model model = new Model();
            Solver solver = new Solver(model, solverType);

            Variable x0 = new Variable("x0");
            Variable x1 = new Variable("x1");
            Variable x2 = new Variable("x2");
            Variable x3 = new Variable("x3");
            Variable x4 = new Variable("x4");
            Variable x5 = new Variable("x5");
            Variable x6 = new Variable("x6");
            Variable h = new Variable("h");

            model.Add(h >= x0);
            model.Add(h >= x1);
            model.Add(h >= x2);
            model.Add(h >= x3);

            model.Add(Math.Sqrt(2.0) * x0 + Math.PI * x1 <= 6 * Math.PI - 1);
            model.Add(Math.Sqrt(3.0) * x2 + Math.E * x5 <= 5.2 * Math.PI);
            model.Add(Math.Sqrt(7.0) * x3 + Math.Cos(1.3) * x4 <= 5.2 * Math.PI - 1);
            model.Add(Math.Sqrt(6.0) * x2 + Math.Cos(2) * x3 >= 1.9 * Math.PI);
            model.Add(Math.Sqrt(4.0) * x5 + Math.Cos(1) * x2 >= 2.1 * Math.PI);
            model.Add(Math.Sqrt(1.7) * x1 + Math.Cos(1.6) * x6 >= 1.6 * Math.PI - 1);

            model.Objective = (x0 + x1 + x2 + x3);
            model.ObjectiveSense = (ObjectiveSense.Maximise);

            solver.Solve();

            Assert(solver.IsProvenOptimal);
            Assert(!solver.IsProvenPrimalInfeasible);
            Assert(!solver.IsProvenDualInfeasible);

            x0.Lower = x0.Upper = x0.Value;
            x1.Lower = x1.Upper = x1.Value;
            x2.Lower = x2.Upper = x2.Value;
            x3.Lower = x3.Upper = x3.Value;

            model.Objective = (1.0 * h);
            model.ObjectiveSense = (ObjectiveSense.Minimise);

            solver.Solve();

            Assert(solver.IsProvenOptimal);
            Assert(!solver.IsProvenPrimalInfeasible);
            Assert(!solver.IsProvenDualInfeasible);

            x4.Lower = x4.Upper = x4.Value;
            x5.Lower = x5.Upper = x5.Value;
            x6.Lower = x6.Upper = x6.Value;

            model.Objective = (x0 + x1 + x2 + x3 + x4 + x5 + x6);
            model.ObjectiveSense = (ObjectiveSense.Minimise);

            solver.Solve();

            Assert(solver.IsProvenOptimal);
            Assert(!solver.IsProvenPrimalInfeasible);
            Assert(!solver.IsProvenDualInfeasible);
        }

        public void SonnetTest28()
        {
            Console.WriteLine("SonnetTest28");

            Model model = new Model();
            Solver solver = new Solver(model, solverType);

            Assert(model.ImportModel("egout.mps"));
            solver.Generate();
            foreach (Variable variable in solver.Variables)
            {
                variable.Type = VariableType.Continuous;
            }

            solver.Solve();

            Assert(solver.IsProvenOptimal);
            Assert(!solver.IsProvenPrimalInfeasible);
            Assert(!solver.IsProvenDualInfeasible);
            Assert(MathExtension.CompareDouble(model.Objective.Value, 149.58876622009566) == 0);

            model.Objective = (Expression.Sum(solver.Variables));
            model.ObjectiveSense = (ObjectiveSense.Minimise);

            foreach (Variable variable in solver.Variables)
            {
                variable.Freeze();
            }

            solver.Solve();

            Assert(solver.IsProvenOptimal);
            Assert(!solver.IsProvenPrimalInfeasible);
            Assert(!solver.IsProvenDualInfeasible);
        }

        public void SonnetTest29()
        {
            Console.WriteLine("SonnetTest29 - single thread stress test");
            SonnetTest29Worker();
        }

        private void SonnetTest29Worker()
        {
            string threadid = System.Threading.Thread.CurrentThread.Name;
            Model model = new Model();
            Solver solver = new Solver(model, solverType);

            var pc = new Microsoft.VisualBasic.Devices.ComputerInfo();
            double memoryGB = (pc.TotalPhysicalMemory + pc.TotalVirtualMemory) / 1073741824.0;
            int n = Math.Min(3, Environment.ProcessorCount + 1);

            // N = 3K, M = 30K, Z=100 requires works with about 1.5GB per thread (n).
            // Let's scale accordingly
            // Note: Arguably only M and Z matter, but we scall also N
            double f = (memoryGB / n) / 1.5;

            // the stress test
            int N = (int)(f * 3000); // number of variables
            int M = (int)(f * 30000); // number of rangeconstraints
            int Z = 100; // number nonzeros per constraint

            Variable x = new Variable();
            Variable[] vars = Variable.NewVariables(N);

            for (int m = 0; m < M; m++)
            {
                if (m % 10000 == 0) Console.WriteLine(string.Format("Thread {0}: {1}", threadid, (1.0 * m / M).ToString("p")));
                Expression expr = new Expression();
                expr.Add(x);

                for (int z = 0; z < Z; z++)
                {
                    int i = (z + m) % N; // always between 0 and N-1
                    expr.Add(vars[i]);
                }

                int available = m;

                string rowName = "MyConstraint(" + m + ")";
                RangeConstraint con = (RangeConstraint)model.Add(
                    -model.Infinity <= expr <= available, rowName);

                expr.Assemble();
                Assert(expr.NumberOfCoefficients == Z + 1);

                expr.Clear();
            }

            solver.Generate();

            Assert(solver.NumberOfConstraints == model.NumberOfConstraints);
            Assert(solver.NumberOfConstraints == M);
            Assert(solver.NumberOfElements == M * (Z + 1));

            Console.WriteLine(string.Format("Thread {0}: finished", threadid));
            model.Clear();
            Console.WriteLine(string.Format("Thread {0}: closed", threadid));
        }

        public void SonnetTest30()
        {
            Console.WriteLine("SonnetTest30 - min(3,(n+1)) multi-processor stress test");
            int n = Math.Min(3, Environment.ProcessorCount + 1);
            Console.WriteLine("Number of threads: " + n);
            System.Threading.Thread[] threads = new System.Threading.Thread[n];
            for (int i = 0; i < n; i++)
            {
                System.Threading.ThreadStart start = new System.Threading.ThreadStart(SonnetTest30Delegate);
                threads[i] = new System.Threading.Thread(start);
                threads[i].Name = "SonnetTest30-thread-" + (i + 1);
                threads[i].Start();
                System.Threading.Thread.Sleep(2000);
            }

            while (true)
            {
                System.Threading.Thread.Sleep(1000);
                bool anyAlive = false;
                for (int i = 0; i < n; i++)
                {
                    if (threads[i].IsAlive)
                    {
                        anyAlive = true;
                        break;
                    }
                }

                if (!anyAlive) break;
            }
        }

        private void SonnetTest30Delegate()
        {
            try
            {
                SonnetTest29Worker();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.ToString());
                if (!System.Diagnostics.Debugger.IsAttached) System.Diagnostics.Debugger.Launch();
                throw;
                // could be out-of-memory... Can we examine the SEHException?
            }
        }

        public void SonnetTest31()
        {
            Console.WriteLine("SonnetTest31 : WarmStart test");

            Model model = new Model();
            Solver solver = new Solver(model, solverType);

            WarmStart emptyWarmStart = solver.GetEmptyWarmStart();

            Variable x0 = new Variable("x0", 0, model.Infinity);
            Variable x1 = new Variable("x1", 0, model.Infinity);

            Objective obj = model.Objective = (3 * x0 + 1 * x1);

            Constraint r0 = x0 * 2 + x1 * 1 <= 10;
            Constraint r1 = x0 * 1 + x1 * 3 <= 15;
            Constraint r2 = x0 * 1 + x1 * 1 <= 10;

            model.Add(r0, "r0");
            model.Add(r1, "r1");
            model.Add(r2, "r2");
            model.Objective = (obj);

            solver.Maximise();
            int iterationCount = solver.IterationCount;

            WarmStart warmStart = solver.GetWarmStart();

            // With Empty WarmStart, should have same number of iterations
            solver.SetWarmStart(emptyWarmStart);
            solver.Maximise();
            Assert(iterationCount == solver.IterationCount);

            // With WARM warmstart, should have zero iterations to optimality!
            solver.SetWarmStart(warmStart);
            solver.Maximise();
            Assert(solver.IterationCount == 0);
        }

        private void BuildChipsModel(Model model)
        {
            Variable xp = new Variable("xp"); // plain chips
            Variable xm = new Variable("xm"); // Mexican chips
            Variable xt = new Variable("xt", 1.0, 2.0); // test

            Objective P = new Objective(2 * xp + 1.5 * xm);
            Constraint slicing = 2 * xp + 4 * xm <= 345;
            Constraint frying = 4 * xp + 5 * xm <= 480;
            Constraint packing = 4 * xp + 2 * xm + xt <= 330;

            model.Objective = (P);
            model.ObjectiveSense = (ObjectiveSense.Maximise);

            model.Add(slicing, "SlicingConstraint");
            model.Add(frying, "FryingConstraint");
            model.Add(packing, "PackingConstraint");
        }

        public void SonnetTest32()
        {
            Console.WriteLine("SonnetTest32 : WarmStart test 2");
            Model model = new Model();
            BuildChipsModel(model);

            Solver solver = new Solver(model, solverType);

            model.GetConstraint("PackingConstraint").Enabled = false;

            solver.Maximise();
            int iterationCount = solver.IterationCount;
            WarmStart warmStart = solver.GetWarmStart();

            model.GetConstraint("PackingConstraint").Enabled = true;

            // but use the old warmstart!
            solver.SetWarmStart(warmStart);

            bool passed = false;
            try
            {
                solver.Maximise();
                passed = true;
            }
            catch
            {
                passed = false;
            }

            // test if this doesnt cause a crash..
            Assert(passed);
        }

        public void SonnetTest33()
        {
            Console.WriteLine("SonnetTest33 : WarmStart test 3");

            Model model = new Model();
            Solver solver = new Solver(model, solverType);
            WarmStart emptyWarmStart = solver.GetEmptyWarmStart();

            Assert(model.ImportModel("brandy.mps"));

            solver.Generate();
            foreach (Variable variable in solver.Variables)
            {
                variable.Type = VariableType.Continuous;
            }

            model.ObjectiveSense = (ObjectiveSense.Minimise);
            solver.OsiSolver.setHintParam(OsiHintParam.OsiDoDualInInitial, false, OsiHintStrength.OsiHintTry);
            solver.OsiSolver.setHintParam(OsiHintParam.OsiDoPresolveInInitial, true, OsiHintStrength.OsiHintTry);
            solver.OsiSolver.setHintParam(OsiHintParam.OsiDoDualInResolve, false, OsiHintStrength.OsiHintTry);
            solver.OsiSolver.setHintParam(OsiHintParam.OsiDoPresolveInResolve, true, OsiHintStrength.OsiHintTry);
            solver.Solve();

            WarmStart warmStart = solver.GetWarmStart();

            // With Empty WarmStart
            solver.SetWarmStart(emptyWarmStart);
            int maxNumIteration;
            solver.OsiSolver.getIntParam(OsiIntParam.OsiMaxNumIteration, out maxNumIteration);
            solver.OsiSolver.setIntParam(OsiIntParam.OsiMaxNumIteration, 2);
            solver.Solve();

            solver.OsiSolver.setIntParam(OsiIntParam.OsiMaxNumIteration, maxNumIteration);

            // With WARM warmstart, should have zero iterations to optimality!
            solver.SetWarmStart(warmStart);
            solver.Resolve();
            Assert(solver.IterationCount == 0);
        }

        public void SonnetTest34()
        {
            Console.WriteLine("SonnetTest34 : WarmStart test 4: add variables to the model between warmstarts");

            Model model = new Model();
            Solver solver = new Solver(model, new OsiCbcSolverInterface());

            solver.OsiSolver.setHintParam(OsiHintParam.OsiDoDualInInitial, false, OsiHintStrength.OsiHintDo);
            solver.OsiSolver.setHintParam(OsiHintParam.OsiDoDualInResolve, false, OsiHintStrength.OsiHintDo);
            BuildChipsModel(model);
            Objective originalObjective = model.Objective;

            solver.Solve();
            WarmStart warmStart = solver.GetWarmStart();

            model.GetConstraint("PackingConstraint").Enabled = false;
            model.Objective = solver.Variables.Sum();
            solver.Resolve();

            model.GetConstraint("PackingConstraint").Enabled = true;
            model.Objective = originalObjective;

            Variable x1 = new Variable();
            Variable x2 = new Variable();
            model.Add(x1 + 2 * x2 <= 5);
            solver.Generate(); // this is necessary, because otherwise these variables are not yet registered when we set the coefficients (below)
            // alternatively, assign the originalObjective *after* setting the coefficients (since orig. obj is currently NOT active in the model)

            originalObjective.SetCoefficient(x1, 1.0);
            originalObjective.SetCoefficient(x2, 1.0);

            solver.SetWarmStart(warmStart);
            solver.Resolve();

            Assert(solver.IterationCount <= 1);
        }

        public void SonnetTest35()
        {
            Console.WriteLine("SonnetTest35 - testing objective values");

            Model model = new Model();

            Variable x0 = new Variable();
            x0.Type = VariableType.Integer;
            Variable x1 = new Variable();
            model.Add(x0 + 2 * x1 <= 3.9);
            model.Objective = (x0 + x1 + 10.0);
            model.ObjectiveSense = (ObjectiveSense.Maximise);

            Solver solver = new Solver(model, solverType);
            solver.Solve();

            Assert(solver.IsProvenOptimal);
            Assert(!solver.IsProvenPrimalInfeasible);
            Assert(!solver.IsProvenDualInfeasible);

            Assert(MathExtension.CompareDouble(model.Objective.Value, 13.45) == 0); // with Constant
            Assert(MathExtension.CompareDouble(model.Objective.Level(), 13.45) == 0); // with Constant
        }

        public void SonnetTest36()
        {
            Console.WriteLine("SonnetTest36 - testing objective values");

            Model model = new Model();
            Solver solver = new Solver(model, solverType);

            Assert(model.ImportModel("MIP-124725.mps")); // added file to project, "Copy Always"

            solver.Minimise();
            Assert(MathExtension.CompareDouble(model.Objective.Value, 124725) == 0);
        }

        public void SonnetTest37()
        {
            if (solverType != SolverType.CbcSolver) return;
            Console.WriteLine("SonnetTest37 - Cbc test set CbcStrategyNull and addCutGenerator");

            Model model = new Model();
            Solver solver = new Solver(model, solverType);

            Assert(model.ImportModel("MIP-124725.mps")); // added file to project, "Copy Always"

            OsiCbcSolverInterface osisolver = solver.OsiSolver as OsiCbcSolverInterface;
            Assert(osisolver != null);

            CbcModel cbcModel = osisolver.getModelPtr();
            cbcModel.setStrategy(new CbcStrategyNull());
            cbcModel.addCutGenerator(new CglProbing());
            Assert(cbcModel.numberCutGenerators() == 1);
            //cbcModel.cutGenerators();

            solver.Minimise();

            Assert(MathExtension.CompareDouble(model.Objective.Value, 124725) == 0);
            //Assert(MathExtension.CompareDouble(model.Objective.Value, 104713.12807881772) == 0);
        }

        public void SonnetTest38()
        {
            if (solverType != SolverType.CbcSolver) return;

            Console.WriteLine("SonnetTest38 - Cbc set CbcStrategyDefault");

            Model model = new Model();
            Solver solver = new Solver(model, solverType);

            Assert(model.ImportModel("MIP-124725.mps")); // added file to project, "Copy Always"
            model.ObjectiveSense = ObjectiveSense.Minimise;

            OsiCbcSolverInterface osisolver = solver.OsiSolver as OsiCbcSolverInterface;
            Assert(osisolver != null);

            CbcModel cbcModel = osisolver.getModelPtr();
            cbcModel.setStrategy(new CbcStrategyDefault(1, 5, 5));
            //cbcModel.strategy().setupCutGenerators(cbcModel);

            solver.AutoResetMIPSolve = true;
            solver.Minimise();

            string message = "Used cut generators: " + string.Join(", ", cbcModel.cutGenerators().Select(generator => generator.generator().GetType().Name));

            Console.WriteLine(message);

            Assert(MathExtension.CompareDouble(model.Objective.Value, 124725) == 0);

            solver.Solve(true);
            Assert(MathExtension.CompareDouble(model.Objective.Value, 104713.12807881772) == 0);
        }

        public void SonnetTest39()
        {
            Console.WriteLine("SonnetTest39 - Test MIP solve followed by solving lp-relaxation");

            Model m = new Model();
            Variable x = new Variable(VariableType.Integer);
            Variable y = new Variable(VariableType.Integer);

            m.Add(0 <= 1.3 * x + 3 * y <= 10, "con1");
            m.Objective = x + 2 * y;

            Solver s = new Solver(m, solverType);
            s.Maximise();

            Console.WriteLine("Status: Optimal? " + s.IsProvenOptimal);
            Console.WriteLine("Status: x = " + x.Value);
            Console.WriteLine("Status: y = " + y.Value);
            Console.WriteLine("Status: obj = " + m.Objective.Value);

            Assert(MathExtension.CompareDouble(m.Objective.Value, 7) == 0);


            s.Maximise(true);
            Console.WriteLine("Status: Optimal? " + s.IsProvenOptimal);
            Console.WriteLine("Status: x = " + x.Value);
            Console.WriteLine("Status: y = " + y.Value);
            Console.WriteLine("Status: obj = " + m.Objective.Value);
            Assert(MathExtension.CompareDouble(m.Objective.Value, 7.6923076923076907) == 0);

            m.GetConstraint("con1").Lower = 4;

            s.Minimise();
            Console.WriteLine("Status: Optimal? " + s.IsProvenOptimal);
            Console.WriteLine("Status: x = " + x.Value);
            Console.WriteLine("Status: y = " + y.Value);
            Console.WriteLine("Status: obj = " + m.Objective.Value);
            Assert(MathExtension.CompareDouble(m.Objective.Value, 3.0) == 0);

            s.Minimise(true);
            Console.WriteLine("Status: Optimal? " + s.IsProvenOptimal);
            Console.WriteLine("Status: x = " + x.Value);
            Console.WriteLine("Status: y = " + y.Value);
            Console.WriteLine("Status: obj = " + m.Objective.Value);
            Assert(MathExtension.CompareDouble(m.Objective.Value, 2.66666666666) == 0);
        }
        

        public static bool EqualsString(string string1, string string2)
        {
            int n1 = string1.Length;
            int n2 = string2.Length;

            int n = Math.Min(n1, n2);
            for (int i = 0; i < n; i++)
            {
                if (!string1[i].Equals(string2[i]))
                {
                    int j = Math.Min(n - i - 1, 5);
                    System.Diagnostics.Debug.WriteLine("The final few characters are not the same:");
                    System.Diagnostics.Debug.WriteLine("first : " + string1.Substring(0, i + j));
                    System.Diagnostics.Debug.WriteLine("second: " + string2.Substring(0, i + j));
                    return false;
                }
            }

            if (n1 != n2) return false;
            return true;
        }
    }

}
