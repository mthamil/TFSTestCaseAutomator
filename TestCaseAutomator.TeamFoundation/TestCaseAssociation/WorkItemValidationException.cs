using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace TestCaseAutomator.TeamFoundation.TestCaseAssociation
{
    /// <summary>
    /// Exception thrown when one or more WorkItem fields fail validation.
    /// </summary>
    public class WorkItemValidationException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WorkItemValidationException"/> class.
        /// </summary>
        /// <param name="invalidFields">A WorkItem's invalid fields.</param>
        public WorkItemValidationException(IEnumerable<Field> invalidFields)
            : base(FormatValidationMessage(invalidFields))
        {
        }

        private static string FormatValidationMessage(IEnumerable<Field> invalidFields)
        {
            return invalidFields.Aggregate(new StringBuilder("Validation Errors:"), (sb, field) => 
                sb.AppendLine()
                  .AppendFormat("Field '{0}' failed with Status: '{1}', Value: '{2}'", field.Name, field.Status, field.Value))
                  .ToString();
        }
    }
}