using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Mono.Cecil;
using Mono.Cecil.Cil;

namespace RiniSharp
{
    interface ILdable
    {
        TypeReference type { get; }

        Instruction[] Ld(ILProcessor ilgen);
    }

    class FieldLd : ILdable
    {
        public TypeReference type { get; private set; }

        private FieldDefinition field { get; set; }

        public FieldLd(FieldDefinition field)
        {
            this.field = field;
            this.type = field.FieldType;
        }

        public Instruction[] Ld(ILProcessor ilgen)
        {
            List<Instruction> insts = new List<Instruction>();

            if (field.IsStatic)
            {
                insts.Add(ilgen.Create(OpCodes.Ldsfld, field));
            }
            else
            {
                insts.Add(ilgen.Create(OpCodes.Ldarg_0));
                insts.Add(ilgen.Create(OpCodes.Ldfld, field));
            }

            return insts.ToArray();
        }
    }

    class PropertyLd : ILdable
    {
        public TypeReference type { get; private set; }

        private PropertyDefinition property { get; set; }

        public PropertyLd(PropertyDefinition property)
        {
            this.property = property;
            this.type = property.PropertyType;
        }

        public Instruction[] Ld(ILProcessor ilgen)
        {
            List<Instruction> insts = new List<Instruction>();

            insts.Add(ilgen.Create(OpCodes.Ldarg_0));
            insts.Add(ilgen.Create(OpCodes.Callvirt, property.GetMethod));

            return insts.ToArray();
        }
    }

    class VariableLd : ILdable
    {
        public TypeReference type { get; private set; }

        private VariableDefinition variable { get; set; }

        public VariableLd(VariableDefinition variable)
        {
            this.variable = variable;
            this.type = variable.VariableType;
        }

        public Instruction[] Ld(ILProcessor ilgen)
        {
            List<Instruction> insts = new List<Instruction>();

            insts.Add(ilgen.Create(OpCodes.Ldloc, variable));

            return insts.ToArray();
        }
    }
}
