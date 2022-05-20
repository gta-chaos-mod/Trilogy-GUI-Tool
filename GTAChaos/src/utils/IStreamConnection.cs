// Copyright (c) 2019 Lordmau5
using GTAChaos.Effects;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GTAChaos.Utils
{
    public class RapidFireEventArgs : EventArgs
    {
        public AbstractEffect Effect { get; set; }
    }

    public interface IStreamConnection
    {
        event EventHandler<EventArgs> OnConnected;
        event EventHandler<EventArgs> OnDisconnected;
        event EventHandler<EventArgs> OnLoginError;
        event EventHandler<RapidFireEventArgs> OnRapidFireEffect;

        int GetRemaining();

        Task<bool> TryConnect();

        bool IsConnected();

        void Kill();

        void SendEffectVotingToGame(bool undetermined = true);

        void SetVoting(Shared.VOTING_MODE votingMode, int untilRapidFire = -1, List<IVotingElement> votingElements = null);

        List<IVotingElement> GetVotedEffects();
    }

    public interface IVotingElement
    {
        int GetId();

        AbstractEffect GetEffect();

        int GetVotes();
    }
}
