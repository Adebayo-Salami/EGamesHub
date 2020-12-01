using System;
using System.Collections.Generic;
using System.Text;

namespace EGamesData.Interfaces
{
    public interface IChallengeService
    {
        bool CreateChallenge(long userId, string challengedEmail, double amountToStake, out string message);
        bool AcceptOrDeclineChallenge(long challengeId, long userId, bool isAccepted, out string message);
        bool PlayChallenge(long challengeId, long userId, out string message);
        bool EndChallenge(long challengeId, out string message);
    }
}
