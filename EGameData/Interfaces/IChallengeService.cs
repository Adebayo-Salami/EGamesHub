using EGamesData.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace EGamesData.Interfaces
{
    public interface IChallengeService
    {
        bool CreateChallenge(long userId, string challengedEmail, double amountToStake, string challengeName, GameType gameType, BrainGameCategory brainGameCategory, out string message);
        bool AcceptOrDeclineChallenge(long challengeId, long userId, bool isAccepted, out string message);
        Challenge PlayChallenge(long challengeId, long userId, out string message);
        bool EndChallenge(long challengeId, List<string> answers, long userId, out string message);
        List<EGamesData.Models.Challenge> GetListOfUserChallenges(long userId);
    }
}
