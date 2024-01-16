
import * as _ from "lodash";

import { Bot } from "./Bot";

export class Bots
{
    private static _all: Bot[] = [];

    private static _my: Bot[] = [];
    private static _foes: Bot[] = [];

    private static _idle: Bot[] = [];
    private static _busy: Bot[] = [];

    private static _idleHarvesters: Bot[] = [];
    private static _idleUpgraders: Bot[] = [];
    private static _idleSuppliers: Bot[] = [];
    private static _idleBuilders: Bot[] = [];
    private static _idleRepairers: Bot[] = [];

    static get all(): Bot[] { return Bots._all; }

    static get my(): Bot[] { return Bots._my; }
    static get foes(): Bot[] { return Bots._foes; }

    static get idle(): Bot[] { return Bots._idle; }
    static get busy(): Bot[] { return Bots._busy; }

    static get idleHarvesters(): Bot[] { return Bots._idleHarvesters; }
    static get idleUpgraders(): Bot[] { return Bots._idleUpgraders; }
    static get idleSuppliers(): Bot[] { return Bots._idleSuppliers; }
    static get idleBuilders(): Bot[] { return Bots._idleBuilders; }
    static get idleRepairers(): Bot[] { return Bots._idleRepairers; }

    static initialize()
    {
        Bots.refresh(true);
    }

    static refresh(all = true)
    {
        if (all)
        {
            Bots._all = _.values<Creep>(Game.creeps).map(c => new Bot(c));
            Bots._my = Bots._all.filter(b => b.my);
            Bots._foes = Bots._all.filter(b => !b.my);
        }

        Bots._idle = Bots._my.filter(b => b.idle);
        Bots._busy = Bots._my.filter(b => !b.idle);

        Bots._idleHarvesters = Bots._idle.filter(b => b.capabilities.canHarvest);
        Bots._idleUpgraders = Bots._idle.filter(b => b.capabilities.canUpgrade);
        Bots._idleSuppliers = Bots._idle.filter(b => b.capabilities.canSupply);
        Bots._idleBuilders = Bots._idle.filter(b => b.capabilities.canBuild);
        Bots._idleRepairers = Bots._idle.filter(b => b.capabilities.canRepair);
    }
}