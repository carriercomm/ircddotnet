﻿/*
 *  The ircd.net project is an IRC deamon implementation for the .NET Plattform
 *  It should run on both .NET and Mono
 * 
 *  Copyright (c) 2009-2010 Thomas Bruderer <apophis@apophis.ch>
 *
 *  This program is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using System.Collections.Generic;
using IrcD.ServerReplies;

namespace IrcD.Commands
{
    public class WhoIs : CommandBase
    {
        public WhoIs(IrcDaemon ircDaemon)
            : base(ircDaemon, "WHOIS")
        { }

        public override void Handle(UserInfo info, List<string> args)
        {
            if (!info.Registered)
            {
                IrcDaemon.Replies.SendNotRegistered(info);
                return;
            }
            if (args.Count < 1)
            {
                IrcDaemon.Replies.SendNeedMoreParams(info);
                return;
            }
            if (!IrcDaemon.Nicks.ContainsKey(args[0]))
            {
                IrcDaemon.Replies.SendNoSuchNick(info, args[0]);
                return;
            }

            var user = IrcDaemon.Nicks[args[0]];
            IrcDaemon.Replies.SendWhoIsUser(info, user);
            if (info.UserPerChannelInfos.Count > 0)
            {
                IrcDaemon.Replies.SendWhoIsChannels(info, user);
            }
            IrcDaemon.Replies.SendWhoIsServer(info, user);
            if (user.AwayMessage != null)
            {
                IrcDaemon.Replies.SendAwayMessage(info, user);
            }

            if (IrcDaemon.Options.IrcMode == IrcMode.Modern)
            {
                IrcDaemon.Replies.SendWhoIsLanguage(info, user);
            }

            if (!info.Modes.HandleEvent(IrcCommandType.Join, info, args))
            {
                return;
            }
            // TODO: User Modes Handle

            //if (user.Mode_O || user.Mode_o)
            //{
            //    SendWhoIsOperator(info, user);
            //}

            IrcDaemon.Replies.SendWhoIsIdle(info, user);
            IrcDaemon.Replies.SendEndOfWhoIs(info, user);
        }
    }
}
