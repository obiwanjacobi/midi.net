using System;
using System.Globalization;
using System.Xml;

namespace CannedBytes
{
    /// <summary>
    /// The Throw class provides static helper methods for method parameter validation.
    /// </summary>
    public static class Throw
    {
        /// <summary>
        /// Checks if the parameter <paramref name="argument"/> is null.
        /// </summary>
        /// <typeparam name="T">Inferred, no need to specify explicitly.</typeparam>
        /// <param name="argument">The parameter value.</param>
        /// <param name="paramName">The parameter name.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="argument"/> is null.</exception>
        public static void IfArgumentNull<T>(T argument, string paramName) where T : class
        {
            if (argument == null)
            {
                throw new ArgumentNullException(paramName);
            }
        }

        /// <summary>
        /// Checks if the parameter <paramref name="argument"/> is null.
        /// </summary>
        /// <param name="argument">The parameter value.</param>
        /// <param name="paramName">The parameter name.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="argument"/> is null.</exception>
        public static void IfArgumentNull(IntPtr argument, string paramName)
        {
            if (argument == IntPtr.Zero)
            {
                throw new ArgumentNullException(paramName);
            }
        }

        /// <summary>
        /// Checks if the parameter <paramref name="argument"/> is null.
        /// </summary>
        /// <typeparam name="T">Inferred, no need to specify explicitly.</typeparam>
        /// <param name="argument">The parameter value.</param>
        /// <param name="paramName">The parameter name.</param>
        /// <param name="message">An exception message.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="argument"/> is null.</exception>
        public static void IfArgumentNull<T>(T argument, string paramName, string message) where T : class
        {
            if (argument == null)
            {
                throw new ArgumentNullException(paramName, message);
            }
        }

        /// <summary>
        /// Checks if the parameter <paramref name="argument"/> is empty.
        /// </summary>
        /// <param name="argument">The parameter value.</param>
        /// <param name="paramName">The parameter name.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="argument"/> is empty.</exception>
        public static void IfArgumentEmpty(Guid argument, string paramName)
        {
            if (argument == Guid.Empty)
            {
                throw new ArgumentException(Properties.Resources.Throw_ArgumentEmpty, paramName);
            }
        }

        /// <summary>
        /// Checks if the parameter <paramref name="argument"/> is null or empty.
        /// </summary>
        /// <param name="argument">The parameter value.</param>
        /// <param name="paramName">The parameter name.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="argument"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="argument"/> is empty.</exception>
        public static void IfArgumentNullOrEmpty(string argument, string paramName)
        {
            if (String.IsNullOrEmpty(argument))
            {
                IfArgumentNull(argument, paramName);

                throw new ArgumentException(Properties.Resources.Throw_ArgumentEmpty, paramName);
            }
        }

        /// <summary>
        /// Checks if the parameter <paramref name="argument"/> is null or empty.
        /// </summary>
        /// <param name="argument">The parameter value.</param>
        /// <param name="paramName">The parameter name.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="argument"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="argument"/> is empty.</exception>
        public static void IfArgumentNullOrEmpty(XmlQualifiedName argument, string paramName)
        {
            IfArgumentNull(argument, paramName);

            if (argument.IsEmpty)
            {
                throw new ArgumentException(Properties.Resources.Throw_ArgumentEmpty, paramName);
            }
        }

        /// <summary>
        /// Checks if the parameter <paramref name="argument"/> is out of range.
        /// </summary>
        /// <typeparam name="T">The parameter data type.</typeparam>
        /// <param name="argument">The parameter value.</param>
        /// <param name="minValue">The parameter's minimal value.</param>
        /// <param name="maxValue">The parameter's maximal value.</param>
        /// <param name="paramName">The parameter name.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="argument"/> is out of range.</exception>
        public static void IfArgumentOutOfRange<T>(IComparable<T> argument, T minValue, T maxValue, string paramName)
        {
            if ((argument.CompareTo(minValue) < 0 || argument.CompareTo(maxValue) > 0))
            {
                throw new ArgumentOutOfRangeException(paramName, argument,
                    String.Format(CultureInfo.InvariantCulture,
                        Properties.Resources.Throw_ArgumentOutOfRange, minValue, maxValue));
            }
        }

        /// <summary>
        /// Tests if the number of characters in <paramref name="argument"/> exceed the <paramref name="maxLength"/>.
        /// </summary>
        /// <param name="argument">The string argument to test. Can be null.</param>
        /// <param name="maxLength">The maximum number of characters allowed for the <paramref name="argument"/>.</param>
        /// <param name="argumentName">The name of the argument.</param>
        /// <exception cref="ArgumentException">Thrown when the number of characters of the <paramref name="argument"/>
        /// exceed the specified <paramref name="maxLength"/>.</exception>
        /// <remarks>This method does nothing if <paramref name="argument"/> is null.</remarks>
        public static void IfArgumentTooLong(string argument, int maxLength, string argumentName)
        {
            if (argument != null && argument.Length > maxLength)
            {
                throw new ArgumentException(
                    String.Format(CultureInfo.InvariantCulture,
                        Properties.Resources.Throw_ArgumentTooLong, argument, maxLength), argumentName);
            }
        }

        /// <summary>
        /// Checks if the parameter <paramref name="argument"/> is a certain <see cref="Type"/>.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> the parameter should be.</typeparam>
        /// <param name="argument">The parameter value.</param>
        /// <param name="paramName">The parameter name.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="argument"/> is not of Type <b>T</b>.</exception>
        public static void IfArgumentNotOfType<T>(object argument, string paramName)
        {
            if (!(argument is T))
            {
                throw new ArgumentException(
                    String.Format(CultureInfo.InvariantCulture,
                        Properties.Resources.Throw_ArgumentNotOfType, typeof(T).FullName), paramName);
            }
        }
    }
}