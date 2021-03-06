![The Outcast](https://raw.githubusercontent.com/evanronnei/OutcastBot/master/OutcastLogo.png)

# The Outcast

The Outcast is bot for the Grim Dawn Discord server developed with [DSharp+](https://github.com/NaamloosDT/DSharpPlus). The main purpose of the bot is for interacting with the `#builds` channel. This includes posting, editing, and deleting builds.

## Commands

* `build <int id>` - Retrieves the build with the given ID.
* `build new` - Creates a new build. Prompts the user to fill in required and optional properties. Posts the build as an embedded object in the `#builds` channel.
* `build edit <int id>` - Allows the build author to edit an existing build.
* `build delete <int id>` - Allows the build author to delete an existing build.
* `build mybuilds <optional: DiscordMember member>` - Displays all builds by the command user, or by the mentioned member.
* `build top <optional: int count(1-5)>` - Displays the top 5 builds, or the top `count` builds.
* `build syncvotes` - Synchronize the votes in the event that the database differs from the channel.
* `build transfer <int id> <DiscordMember newOwner>` - Transfers ownership of a build to another user.
* `tag <string key>` - Retrives the value with the given key.
* `tag submit <string key> <sting[] value>` - Submits a new tag for moderator approval in `#moderation`
* `tag list` - Retrieves a list of all tags.
* `tag new <string key> <string[] value>` - Creates a new tag with `key` and `value`. Requires `ManageChannels` permissions to execute.
* `tag edit <string key>` - Edits an existing tag `key`. Requires `ManageChannels` permissions to execute.
* `tag delete <string key>` - Deletes and existing tag `key`. Requires `ManageChannels` permissions to execute.
* `credits` - Displays the bot author and source code link in and embedded object.
* `bug` - Links to the issues page here.
* `f <optional: string text>` - Pays respects. If `text` exists then it pays respects to `text`.
* `quote <ulong id>` - Quotes a Discord message with the given ID
* `mobile <DiscordMember member>` - Responds with a composite image of `member`'s avatar and  [MobileDiscord.png](https://github.com/evanronnei/OutcastBot/blob/master/OutcastBot/Images/MobileDiscord.png)

## Events

* Responds to [grimtools build](http://www.grimtools.com/calc/) links with information about the build in an embedded object.
* Thinkematics™
* Reacts to any instances of crab, crab commando, or crabmando with a 🦀 emoji.
* Keeps track of votes on builds.
* Removes deleted builds from the database.
* Posts deleted messages from `#trade` and `#searching-players` to the `#broomcloset` channel in an embedded object.

## Join us on Discord

[![Join Grim Dawn on Discord](https://raw.githubusercontent.com/evanronnei/OutcastBot/master/GrimDawnJoinBanner.png)](https://discord.gg/2FYWt2B)
