// Copyright (c) 2019 Lordmau5
using GtaChaos.Models.Effects.@abstract;
using System;
using System.Collections.Generic;
using TwitchLib.Client;

namespace GtaChaos.Models.Utils
{
    public class RapidFireEventArgs : EventArgs
    {
        public AbstractEffect Effect { get; set; }
    }

    public interface ITwitchConnection
    {
        event EventHandler<RapidFireEventArgs> OnRapidFireEffect;

        int GetRemaining();

        TwitchClient GetTwitchClient();

        void Kill();

        void SendEffectVotingToGame(bool undetermined = true);

        void SetVoting(int votingMode, int untilRapidFire = -1, List<IVotingElement> votingElements = null);

        List<IVotingElement> GetMajorityVotes();
    }

    public interface IVotingElement
    {
        int GetId();

        AbstractEffect GetEffect();
    }
}
