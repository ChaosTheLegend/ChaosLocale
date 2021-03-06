using System.Collections;
using System.Collections.Generic;
using ChaosLocale.Scripts.Core.Data;
using Locale.Scripts;
using UnityEngine;
using UnityEngine.Serialization;

namespace Localization
{
	public class LanguageManager : MonoBehaviour
	{

		public static LanguageManager Instance { get; private set; }

		public void Awake()
		{
			Instance = this;
		}
		
		private const string DATABASE_PATH = @"Assets/LanguageDatabase.asset";

		
		/// <summary>
		/// Gets the meaning.
		/// </summary>
		/// <returns>The meaning.</returns>
		/// <param name="sourceText">Word for translation.</param>
		/// <param name="targetLanguage">Target language.</param>
		public string GetMeaning(string sourceText, Languages targetLanguage){
			return Database.GetMeaning(sourceText, targetLanguage);
		}
		
		public string GetRegularMeaning(string sourceText, Languages targetLanguage, params Translation.RegularTranslation[] expressions){
			return Database.GetRegularTranslation(sourceText, targetLanguage, expressions);
		}
		
		
		public List<RegularExpression> GetRegularExpressions(string sourceText)
		{
			sourceText = sourceText.ToLower ();
			var word = Database.GetDB().Find (x => x.word.Equals (sourceText));
			if (word == null)
			{
				Debug.LogError($"There is no word with the key {sourceText}");
				return null;
			}

			if (word.hasRegularExpression == false)
			{
				Debug.LogError($"Word with the key {sourceText} does not support regular expressions");
				return null;
			}			

			return word.regularExpressions;
		}
		
		[SerializeField] private LanguageDatabase database;
		private LanguageDatabase Database
		{
			get
			{
				if (database == null)
				{
					database = Resources.Load<LanguageDatabase>(DATABASE_PATH);
				}
				return database;
			}
		}
	}
}
