using System.Collections.Generic;
using Zxcvbn.Matcher;

namespace Zxcvbn
{
    /// <summary>
    /// Interface that matcher factories must implement. Matcher factories return a list of the matchers
    /// that will be used to evaluate the password
    /// </summary>
    public interface IMatcherFactory
    {
        /// <summary>
        /// <para>Create the matchers to be used by an instance of Zxcvbn. </para>
        ///
        /// <para>This function will be called once per each password being evaluated, to give the opportunity to provide
        /// different user inputs for each password. Matchers that are not dependent on user inputs should ideally be created
        /// once and cached so that processing (e.g. dictionary loading) will only have to be performed once, these cached
        /// matchers plus any user input matches would then be returned when CreateMatchers is called.</para>
        /// </summary>
        /// <param name="userInputs">List of per-password user information for this invocation</param>
        /// <returns>An enumerable of <see cref="IMatcher"/> objects that will be used to pattern match this password</returns>
        IEnumerable<IMatcher> CreateMatchers(IEnumerable<string> userInputs);
    }
}