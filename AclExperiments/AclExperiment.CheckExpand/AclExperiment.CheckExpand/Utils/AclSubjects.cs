using AclExperiment.CheckExpand.Models;

namespace AclExperiment.CheckExpand.Utils
{
    public static class AclSubjects
    {
        /// <summary>
        /// Converts a given string to a <see cref="AclSubject"/>, which is either a <see cref="AclSubjectId"/> or a <see cref="AclSubjectSet"/>.
        /// </summary>
        /// <param name="s">Subject String in Google Zanzibar Notation</param>
        /// <returns>The <see cref="AclSubject"/></returns>
        public static AclSubject SubjectFromString(string s)
        {
            if (s.Contains('#'))
            {
                return AclSubjectSet.FromString(s);
            }

            return AclSubjectId.FromString(s);
        }

        public static string SubjectToString(AclSubject s)
        {
            switch (s)
            {
                case AclSubjectId subjectId:
                    return subjectId.FormatString();
                case AclSubjectSet subjectSet:
                    return subjectSet.FormatString();
                default:
                    throw new InvalidOperationException($"Cannot format Subject Type '{s.GetType().Name}'");
            }
        }
    }
}
