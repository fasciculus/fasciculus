
import * as _ from "lodash";

import { Bot } from "./Bot";

export class Bots
{
    static get all(): Bot[] { return _.values<Creep>(Game.creeps).map(c => new Bot(c)); }

    static get my(): Bot[] { return Bots.all.filter(b => b.my); }
    static get enemies(): Bot[] { return Bots.all.filter(b => !b.my); }

    static get idle(): Bot[] { return Bots.my.filter(b => b.idle); }
    static get busy(): Bot[] { return Bots.my.filter(b => !b.idle); }

    static get idleHarvesters(): Bot[] { return Bots.idle.filter(b => b.capabilities.canHarvest); }
    static get idleUpgraders(): Bot[] { return Bots.idle.filter(b => b.capabilities.canUpgrade); }
    static get idleSuppliers(): Bot[] { return Bots.idle.filter(b => b.capabilities.canSupply); }
    static get idleBuilders(): Bot[] { return Bots.idle.filter(b => b.capabilities.canBuild); }
}