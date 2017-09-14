![The Outcast](https://raw.githubusercontent.com/evanronnei/OutcastBot/master/OutcastLogo.png)

# The Outcast

The Outcast is bot for the Grim Dawn Discord server developed with [DSharp+](https://github.com/NaamloosDT/DSharpPlus). The main purpose of the bot is for interacting with the `#builds` channel. This includes posting, editting, and deleting builds.

## Commands

* `build <int id>` - Retrieves the build with the given ID.
* `build new` - Creates a new build. Prompts the user to fill in required and optional properties. Posts the build as an embedded object in the `#builds` channel.
* `build edit <int id>` - Allows the build author to edit an existing build.
* `build delete <int id>` - Allows the build author to delete an existing build.
* `build mybuilds <optional: string userMention>` - Displays all builds by the command user, or by the mentioned user.
* `build top <optional: int count(1-10)` - Displays the top 5 builds, or the top `count` builds.
* `bug` - Links to the issues page here.
* `f` - Pays respects.

## Events

* Reacts to any instances of crab, crab commando, or crabmando with a 🦀 emoji.
* Keeps track of votes on builds.
* Removes deleted builds from the database.
* Posts deleted messages from `#trade` and `#searching-players` to the `#broomcloset` channel in an embedded object.

[![Join Grim Dawn on Discord](https://raw.githubusercontent.com/evanronnei/OutcastBot/master/GrimDawnJoinBanner.png)](https://discord.gg/2FYWt2B)
