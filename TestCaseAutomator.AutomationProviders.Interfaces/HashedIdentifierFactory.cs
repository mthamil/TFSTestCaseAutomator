using System;
using System.Security.Cryptography;
using System.Text;

namespace TestCaseAutomator.AutomationProviders.Interfaces
{
	/// <summary>
	/// A default <see cref="ITestIdentifierFactory"/> that creates a deterministic and reproducable identifier from
	/// the hashed bytes of a test's name.
	/// </summary>
	public class HashedIdentifierFactory : ITestIdentifierFactory
	{
		/// <summary>
		/// Initializes a new <see cref="HashedIdentifierFactory"/> with a default hash algorithm.
		/// </summary>
		public HashedIdentifierFactory() 
			: this(() => new SHA1CryptoServiceProvider())
		{
		}

		/// <summary>
		/// Initializes a new <see cref="HashedIdentifierFactory"/>.
		/// </summary>
		/// <param name="hasherFactory">Creates the hash algorithm to use</param>
		public HashedIdentifierFactory(Func<HashAlgorithm> hasherFactory)
		{
			_hasherFactory = hasherFactory;
		}

		/// <see cref="ITestIdentifierFactory.CreateIdentifier"/>
		public Guid CreateIdentifier(string identifyingInput)
		{
			using (var hasher = _hasherFactory())
			{
				var bytes = new byte[16];
				Array.Copy(hasher.ComputeHash(Encoding.Unicode.GetBytes(identifyingInput)), bytes, bytes.Length);
				return new Guid(bytes);
			}
		}

		private readonly Func<HashAlgorithm> _hasherFactory;
	}
}