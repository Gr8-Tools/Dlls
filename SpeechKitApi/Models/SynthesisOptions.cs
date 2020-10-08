// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using System.Collections.Generic;
using System.Linq;
using SpeechKitApi.Utils;

namespace SpeechKitApi.Models
{
    /// <summary>
    /// Speech synthesis options.
    /// </summary>
    public class SynthesisOptions
    {
        /// <summary>
        /// The text to produce speech for. For homographs, use a + before the stressed syllable: def+ect.
        /// </summary>
        public string Text { get; }

        /// <summary>
        /// The speed (tempo) of the synthesized speech. 
        /// </summary>
        public float Speed { get; }
        
        /// <summary>
        /// Accessible folder identity
        /// </summary>
        public string FolderId { get; }
        
        /// <summary>
        /// Exteranl synthesis options
        /// </summary>
        public SynthesisExternalOptions ExteranlOptions { get; set; }

        /// <summary>
        /// Create new speech synthesis options.
        /// </summary>
        /// <param name="folderIdentity">Идентификатор каталога, к которому у вас есть доступ.</param>
        /// <param name="text">The text to produce speech for.</param>
        /// <param name="speed">The speed (tempo) of the synthesized speech. Must be in range from 0.1 to 3.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public SynthesisOptions(string text, float speed = 0.75f, string folderIdentity = "")
        {
            if (text == null)
                throw new ArgumentNullException(nameof(text));
            if (string.IsNullOrWhiteSpace(text))
                throw new ArgumentException(nameof(text));
            
            FolderId = folderIdentity;
            Text = text;
            
            Speed = speed.Clamp(0.1f, 3f);;
        }

        /// <summary>
        /// Создает опции по текстовому набору и общим дополнительным опциям
        /// </summary>
        public static IEnumerable<SynthesisOptions> Create(IEnumerable<string> texts, SynthesisExternalOptions externalOptions, float speed = 0.75f, string folderIdentity = "")
        {
            return texts.Select(text => new SynthesisOptions(text, speed, folderIdentity)
            {
                ExteranlOptions = externalOptions
            });
        }
    }
}
