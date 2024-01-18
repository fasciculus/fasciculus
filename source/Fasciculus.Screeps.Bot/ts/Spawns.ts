import * as _ from "lodash";

import { Rooms } from "./Rooms";
import { Sources } from "./Sources";
import { Creeps } from "./Creeps";
import { Names } from "./Names";
import { Bodies } from "./Bodies";
import { Controllers } from "./Controllers";
import { Constructions } from "./Constructions";
import { CreepState, CreepType } from "./Enums";
import { CreepBaseMemory, UpgraderMemory, WellerMemory } from "./Memories";
import { Wells } from "./Wells";

export class Spawns
{
    private static _my: StructureSpawn[] = [];
    private static _idle: StructureSpawn[] = [];

    static get my(): StructureSpawn[] { return Spawns._my; }
    static get idle(): StructureSpawn[] { return Spawns._idle; }

    private static myOf(room: Room): StructureSpawn[]
    {
        return room.find<FIND_MY_SPAWNS, StructureSpawn>(FIND_MY_SPAWNS)
    }

    static initialize()
    {
        Spawns._my = _.flatten(Rooms.all.map(Spawns.myOf));
        Spawns._idle = Spawns._my.filter(s => s.spawning === null);
    }

    static run()
    {
        var spawn = Spawns.bestSpawn();

        if (!spawn) return;

        var type = Spawns.nextType(spawn);

        if (!type) return;

        switch (type)
        {
            case CreepType.Weller: Spawns.spawnWeller(spawn); break;
            case CreepType.Supplier: Spawns.spawnSupplier(spawn); break;
            case CreepType.Upgrader: Spawns.spawnUpgrader(spawn); break;
            case CreepType.Builder: Spawns.spawnBuilder(spawn); break;
        }
    }

    private static bestSpawn(): StructureSpawn | undefined
    {
        var spawns = Spawns.idle;

        if (spawns.length == 0) return undefined;

        return spawns.sort((a, b) => b.room.energyAvailable - a.room.energyAvailable)[0];
    }

    private static nextType(spawn: StructureSpawn): CreepType | undefined
    {
        var wellerCount = Creeps.countOf(CreepType.Weller);
        var sourceCount = Sources.all.length;

        if (wellerCount == 0 && sourceCount > 0) return CreepType.Weller;

        var supplierCount = Creeps.countOf(CreepType.Supplier);

        if (supplierCount == 0 && wellerCount > 0) return CreepType.Supplier;

        var upgraderCount = Creeps.countOf(CreepType.Upgrader);

        if (wellerCount > 0 && supplierCount > 0 && upgraderCount == 0) return CreepType.Upgrader;

        var room = spawn.room;

        if (room.energyAvailable < room.energyCapacityAvailable) return undefined;

        if (wellerCount > supplierCount) return CreepType.Supplier;
        if (Spawns.needMoreWellers()) return CreepType.Weller;

        var siteCount = Constructions.my.length;
        var builderCount = Creeps.countOf(CreepType.Builder);
        var requiredBuilderCount = siteCount > 0 ? 1 + Math.floor(Constructions.notWalls.length / 3) : 0;
        var requiredSupplierCount = wellerCount + Math.floor(builderCount / 2 + upgraderCount / 2);

        if (supplierCount < requiredSupplierCount) return CreepType.Supplier;
        if (builderCount < requiredBuilderCount) return CreepType.Builder;

        if (upgraderCount < 2)
        {
            var upgradeForce = _.flatten(Creeps.ofType(CreepType.Upgrader).map(c => c.body)).map(d => d.type).filter(t => t == WORK).length;

            if (upgradeForce < 15) return CreepType.Upgrader;
        }
        
        return undefined;
    }

    private static needMoreWellers(): boolean
    {
        for (let well of Wells.all)
        {
            if (well.assignedWork >= 10) continue;
            if (well.assignees.length < well.slots.length) return true;
        }

        return false;
    }

    private static spawnWeller(spawn: StructureSpawn)
    {
        var memory: CreepBaseMemory = { type: CreepType.Weller, state: CreepState.Idle };

        Spawns.spawnCreep(spawn, memory);
    }

    private static spawnSupplier(spawn: StructureSpawn)
    {
        var memory: CreepBaseMemory = { type: CreepType.Supplier, state: CreepState.Idle };

        Spawns.spawnCreep(spawn, memory);
    }

    private static spawnUpgrader(spawn: StructureSpawn)
    {
        var controller = Spawns.findBestController();

        if (!controller) return;

        var memory: UpgraderMemory = { type: CreepType.Upgrader, state: CreepState.Idle, controller: controller.id };

        Spawns.spawnCreep(spawn, memory);
    }

    private static findBestController(): StructureController | undefined
    {
        var controllers = Controllers.all;

        if (controllers.length == 0) return undefined;

        var counts: { [id: Id<StructureController>]: number } = {};
        var assignees = Creeps.ofType(CreepType.Upgrader);

        controllers.forEach(c => counts[c.id] = 0);

        for (let assignee of assignees)
        {
            let id = (assignee.memory as UpgraderMemory).controller;

            if (!id || !_.has(counts, id)) continue;

            ++counts[id];
        }

        var bestController = controllers[0];
        var bestCount = counts[bestController.id];

        for (let i = 1, n = controllers.length; i < n; ++i)
        {
            let controller = controllers[i];
            let count = counts[controller.id];

            if (count < bestCount)
            {
                bestController = controller;
                bestCount = count;
            }
        }

        return bestController;
    }

    private static spawnBuilder(spawn: StructureSpawn)
    {
        var memory: CreepBaseMemory = { type: CreepType.Builder, state: CreepState.Idle };

        Spawns.spawnCreep(spawn, memory);
    }

    static spawnCreep(spawn: StructureSpawn, memory: CreepBaseMemory): ScreepsReturnCode
    {
        let energy = spawn.room.energyAvailable;
        let body = Bodies.create(memory.type, energy);

        if (!body) return ERR_NOT_ENOUGH_ENERGY;

        let name = Names.next(memory.type);
        let opts: SpawnOptions = { memory };

        return spawn.spawnCreep(body, name, opts);
    }
}