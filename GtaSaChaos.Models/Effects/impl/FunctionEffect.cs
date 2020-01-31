// Copyright (c) 2019 Lordmau5
using GtaChaos.Models.Effects.@abstract;
using GtaChaos.Models.Utils;

namespace GtaChaos.Models.Effects.impl
{
    public class FunctionEffect : AbstractEffect
    {
        private string type = "effect";
        private readonly string function;

        public FunctionEffect(Category category, string description, string word, string _function, int duration = -1, float multiplier = 1.0f)
            : base(category, description, word, duration, multiplier)
        {
            function = _function;
        }

        public FunctionEffect SetType(string type)
        {
            this.type = type;
            return this;
        }

        public override string GetAudioFile()
        {
            string file = base.GetAudioFile();

            return string.IsNullOrWhiteSpace(file) ? function : file;
        }

        public override void RunEffect(int seed = -1, int _duration = -1)
        {
            base.RunEffect(seed, _duration);

            SendEffectToGame("set_seed", seed == -1 ? RandomHandler.Next(9999999).ToString() : seed.ToString());
            SendEffectToGame(type, function, (_duration == -1 ? Duration : _duration), "");
        }
    }
}
