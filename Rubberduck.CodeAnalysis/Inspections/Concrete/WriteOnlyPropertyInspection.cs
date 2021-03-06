using System.Collections.Generic;
using System.Linq;
using Rubberduck.Inspections.Abstract;
using Rubberduck.Inspections.Results;
using Rubberduck.Parsing.Inspections.Abstract;
using Rubberduck.Resources.Inspections;
using Rubberduck.Parsing.Symbols;
using Rubberduck.Parsing.VBA;
using Rubberduck.Inspections.Inspections.Extensions;

namespace Rubberduck.Inspections.Concrete
{
    public sealed class WriteOnlyPropertyInspection : InspectionBase
    {
        public WriteOnlyPropertyInspection(RubberduckParserState state)
            : base(state) { }

        protected override IEnumerable<IInspectionResult> DoGetInspectionResults()
        {
            var setters = State.DeclarationFinder.UserDeclarations(DeclarationType.Property | DeclarationType.Procedure)
                .Where(item => 
                       (item.Accessibility == Accessibility.Implicit || 
                        item.Accessibility == Accessibility.Public || 
                        item.Accessibility == Accessibility.Global)
                    && State.DeclarationFinder.MatchName(item.IdentifierName).All(accessor => accessor.DeclarationType != DeclarationType.PropertyGet))
                .Where(result => !result.IsIgnoringInspectionResultFor(AnnotationName))
                .GroupBy(item => new {item.QualifiedName, item.DeclarationType})
                .Select(grouping => grouping.First()); // don't get both Let and Set accessors

            return setters.Select(setter =>
                new DeclarationInspectionResult(this,
                                                string.Format(InspectionResults.WriteOnlyPropertyInspection, setter.IdentifierName),
                                                setter));
        }
    }
}
