using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NavyFish
{
    class DockingFunctionsHelper
    {
		static bool Check_KSPAssembly(CustomAttributeData atr)
		{
			if(atr.ConstructorArguments.Count != 3 && atr.ConstructorArguments.Count != 4)
				return false;
            if((string)atr.ConstructorArguments[0].Value != "DockingFunctions")
                return false;
			if((int)atr.ConstructorArguments[1].Value < 1)
                return false;
            return true;
		}

        static Assembly FindAssembly()
        {
            foreach(var x in AssemblyLoader.loadedAssemblies)
            {
				foreach(CustomAttributeData customAttribute in x.assembly.CustomAttributes)
				{
				    if(customAttribute.AttributeType.Name == "KSPAssembly")
					{
                        if(Check_KSPAssembly(customAttribute))
                            return x.assembly;
					}
                }
            }

            return null;
        }

        static bool init = false;
        static Assembly assembly;
        static Type type;
        static MethodInfo method;

        public static void Initialize()
        {
            if(init)
                return;

            assembly = FindAssembly();

            if(assembly != null)
            {
                type = assembly.GetType("DockingFunctions.IDockable");
                method = type.GetMethod("IsReadyFor");
            }

            init = true;
        }

        // true if target is ready for docking with port
        public static bool IsReadyFor(Part target, Part port)
        {
            if(assembly == null)
                return true;

            object portc = port.GetComponent(type);
            object targetc = target.GetComponent(type);

            if((portc != null) && (targetc != null))
                return (bool)method.Invoke(targetc, new object[] { portc });

            return false;
        }
    }
}
