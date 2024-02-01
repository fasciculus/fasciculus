
import { Vector } from "./Common";
import { Profiler } from "./Profiling";
import { Chambers } from "./Rooming";
import { Scheduler } from "./Scheduler";

function roomCallback(name: string): boolean | CostMatrix
{
    return new PathFinder.CostMatrix();
}

function pos2str(p: RoomPosition): string
{
    return p.roomName + "_" + p.x + "_" + p.y;
}

function findStructures<T extends AnyStructure>(room: Room, type: string): Vector<T>
{
    const opts: FilterOptions<FIND_STRUCTURES> = { filter: { structureType: type } };

    return Vector.from(room.find<T>(FIND_STRUCTURES, opts));
}

function findAllStructures(room: Room, types: Set<string>): Vector<AnyStructure>
{
    return Vector.from(room.find<AnyStructure>(FIND_STRUCTURES)).filter(s => types.has(s.structureType));
}

export const loop = function ()
{
    Profiler.start();

    Scheduler.initialize();
    Scheduler.run();

    //const spawn: StructureSpawn = Game.spawns["Spawn1"];
    //const flag: Flag = Game.flags["U1"];
    //const opts: PathFinderOpts = { roomCallback };
    //const pfp: PathFinderPath = PathFinder.search(spawn.pos, flag.pos, opts);
    //const path: Vector<RoomPosition> = Vector.from(pfp.path);

    //console.log(`incomplete: ${pfp.incomplete}, cost: ${pfp.cost}, ops: ${pfp.ops}, length: ${path.length}`);
    //console.log(`  ${path.map(pos2str).toArray()}`);

    const room: Room = Chambers.get("W7N3")?.room!;
    const spawns = findStructures<StructureSpawn>(room, STRUCTURE_SPAWN);
    const roads = findStructures<StructureRoad>(room, STRUCTURE_ROAD);
    const types: Set<string> = new Set([STRUCTURE_SPAWN, STRUCTURE_ROAD]);
    const both: Vector<AnyStructure> = findAllStructures(room, types);

    console.log(`spawns: ${spawns.length}, roads: ${roads.length}, both: ${both.length}`);

    const obstacleTypes: Set<string> = new Set([STRUCTURE_SPAWN, STRUCTURE_WALL, STRUCTURE_EXTENSION, STRUCTURE_LINK, STRUCTURE_STORAGE,
        STRUCTURE_TOWER, STRUCTURE_OBSERVER, STRUCTURE_POWER_SPAWN, STRUCTURE_POWER_BANK, STRUCTURE_LAB, STRUCTURE_TERMINAL, STRUCTURE_NUKER,
        STRUCTURE_FACTORY, STRUCTURE_INVADER_CORE]);
    const obstacles: Vector<AnyStructure> = findAllStructures(room, obstacleTypes);

    console.log(`obstacles: [${obstacles.toArray()}]`);

    Profiler.stop();
}