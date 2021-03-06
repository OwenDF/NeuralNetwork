﻿using System.Collections.Generic;
using System.Linq;
using Emotion.Detector.Lexicons.Data;
using Emotion.Detector.Lexicons.Extensions;
using Emotion.Detector.Lexicons.Interfaces;
using Emotion.Detector.Lexicons.Repositories;
using log4net;

namespace Emotion.Detector.Lexicons.Detectors
{
    public class EmotionDetector : IEmotionDetector
    {
        private readonly ILog _log;
        private readonly WordRepository _repository;
        private readonly NegationManager _negationManager;

        public EmotionDetector(ILog log, WordRepository repository, NegationManager negationManager)
        {
            _log = log;
            _repository = repository;
            _negationManager = negationManager;
        }

        public EmotionData Detect(string text)
        {
            var words = text.GetWordsFromText();
            var emotions = _repository.GetEmotions(words);

            AmendNegations(emotions);

            var foundEmotions = emotions.Where(e => e.emotion != null);
            return foundEmotions.Select(e => e.emotion).GetOverallEmotion();
        }

        // Don't worry, this will DEFINITELY detect sarcasm.
        private void AmendNegations(IReadOnlyList<(string word, EmotionData emotion)> emotions)
        {
            for (var i = 1; i < emotions.Count; i++)
            {
                if (emotions[i].emotion == null) continue;

                if (_negationManager.IsNegation(emotions[i - 1].word))
                {
                    emotions[i].emotion.Invert();
                }
            }
        }
    }
}
