using SF_s_Later_Join.Commands;
using Smod2;
using Smod2.API;
using Smod2.Attributes;
using System;
using System.Collections.Generic;
using System.Timers;

namespace SF_s_Later_Join {
  [PluginDetails(
        author = "ShingekiNoRex, storm37000, @Second_Fry",
        name = "SF's Later Join",
        description = "Allow those who join just after round start to spawn",
        id = "sf.later.join",
        version = "1.3.0",
        SmodMajor = 3,
        SmodMinor = 1,
        SmodRevision = 7
        )]
  public class LaterJoin : Plugin {
    private bool isDisabled = false;
    private List<Role> enabledSCPs = new List<Role>();
    private List<int> respawnQueue = new List<int>();
    private LJEventHandler ljEventHandler = null;

    public override void OnDisable() {
      this.Debug("[OnDisable] Disabling");

      this.ResetEnabledSCPs();
      this.ResetRespawnQueue();

      this.Info("[OnDisable] Disabled");
    }

    public override void OnEnable() {
      this.Debug("[OnEnable] Enabling");

      this.CheckIfDisabled();
      this.PopulateEnabledSCPs();
      this.PopulateRespawnQueue();

      this.Info("[OnEnable] Enabled");
    }

    public override void Register() {
      this.ljEventHandler = new LJEventHandler(this);
      this.AddEventHandlers(this.ljEventHandler);

      this.AddConfig(new Smod2.Config.ConfigSetting("sf_lj_disable", false, true, "Disables Second_Fry's Later Join"));
      this.AddConfig(new Smod2.Config.ConfigSetting("sf_lj_time", 120, true, "Amount of time for player to join and still spawn after round start"));
      this.AddConfig(new Smod2.Config.ConfigSetting("sf_lj_explore", false, true, "Allows player to explore the map before game start"));

      this.AddCommand("sf_lj_disable", new DisableCommand(this));
      this.AddCommand("sf_lj_enable", new EnableCommand(this));
      this.AddCommand("sf_lj_reload", new ReloadCommand(this));
    }

    public void CheckIfDisabled() {
      IConfigFile config = ConfigManager.Manager.Config;
      this.isDisabled = config.GetBoolValue("sf_lj_disable", false);
    }

    private void PopulateEnabledSCPs() {
      string[] prefixes = {
        "scp049",
        "scp079",
        "scp096",
        "scp106",
        "scp173",
        "scp939_53",
        "scp939_89"
      };
      IConfigFile config = ConfigManager.Manager.Config;

      foreach (string prefix in prefixes) {
        bool isDisabled = config.GetBoolValue(prefix + "_disable", false);
        if (isDisabled) {
          continue;
        }

        Role role = LaterJoin.ConvertSCPPrefixToRoleID(prefix);
        if (role == Role.UNASSIGNED) {
          this.Error("Trying to convert unknown prefix: " + prefix);
          continue;
        }

        int amount = config.GetIntValue(prefix + "_amount", 1);
        while (amount > 0) {
          this.enabledSCPs.Add(role);
          amount--;
        }
      }
    }

    private void ResetEnabledSCPs() {
      this.enabledSCPs.Clear();
    }

    private void PopulateRespawnQueue() {
      IConfigFile config = ConfigManager.Manager.Config;
      string teamIDs = config.GetStringValue("team_respawn_queue", "4014314031441404134040143");
      foreach (char teamID in teamIDs) {
        this.respawnQueue.Add((int)Char.GetNumericValue(teamID));
      }
    }

    private void ResetRespawnQueue() {
      this.respawnQueue.Clear();
    }

    private static Role ConvertSCPPrefixToRoleID(string prefix) {
      switch (prefix) {
        case "scp049":
          return Role.SCP_049;
        case "scp079":
          return Role.SCP_079;
        case "scp096":
          return Role.SCP_096;
        case "scp106":
          return Role.SCP_106;
        case "scp173":
          return Role.SCP_173;
        case "scp939_53":
          return Role.SCP_939_53;
        case "scp939_89":
          return Role.SCP_939_89;
      }

      return Role.UNASSIGNED;
    }

    public bool GetIsDisabled() {
      return this.isDisabled;
    }

    public LaterJoin SetIsDisabled(bool value) {
      this.isDisabled = value;
      return this;
    }

    public List<Role> GetEnabledSCPs() {
      return this.enabledSCPs;
    }

    public List<int> GetRespawnQueue() {
      return this.respawnQueue;
    }

    public LJEventHandler GetLJEventHandler() {
      return this.ljEventHandler;
    }
  }
}
