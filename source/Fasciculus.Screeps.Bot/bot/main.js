
const CREEP_BODY_300 = [WORK, CARRY, MOVE, MOVE];
const CREEP_BODY_400 = [WORK, WORK, CARRY, CARRY, MOVE, MOVE];

function getCreepCount()
{
    var result = 0;

    for (var name in Game.creeps)
    {
        ++result;
    }

    return result;
}

function findCreepBody(room)
{
    var energyCapacity = room.energyCapacityAvailable;
    var energy = room.energyAvailable;

    if (getCreepCount() == 0)
    {
        energyCapacity = 300;
    }

    if (energy < energyCapacity)
    {
        return undefined;
    }

    console.log(`${energy}/${energyCapacity}`);

    if (energyCapacity >= 400)
    {
        return CREEP_BODY_400;
    }

    if (energyCapacity >= 300)
    {
        return CREEP_BODY_300;
    }

    return undefined;
}

function createCreepName(prefix)
{
    var index = 1;
    var name = `${prefix}${index}`;

    while (Game.creeps[name])
    {
        ++index;
        name = `${prefix}${index}`;
    }

    return name;
}

function findMyExtensions(room)
{
    var result = [];
    var structures = room.find(FIND_MY_STRUCTURES);

    for (var structure of structures)
    {
        if (structure.structureType == STRUCTURE_EXTENSION)
        {
            result.push(structure);
        }
    }

    return result;
}

function findEnergyTargets(creep)
{
    var result = [];
    var room = creep.room;
    var spawns = room.find(FIND_MY_SPAWNS);
    var extensions = findMyExtensions(room);

    for (var spawn of spawns)
    {
        if (spawn.store.getFreeCapacity(RESOURCE_ENERGY) > 0)
        {
            result.push(spawn);
        }
    }

    for (var extension of extensions)
    {
        if (extension.store.getFreeCapacity(RESOURCE_ENERGY) > 0)
        {
            result.push(extension);
        }
    }

    return result;
}

function findConstruction(creep)
{
    var result = undefined;
    var constructions = creep.room.find(FIND_MY_CONSTRUCTION_SITES);

    if (constructions.length > 0)
    {
        result = creep.pos.findClosestByPath(constructions);
    }

    return result;
}

function findRepairables(room)
{
    var result = [];
    var structures = room.find(FIND_STRUCTURES);

    for (var structure of structures)
    {
        if (structure.hits < structure.hitsMax)
        {
            result.push(structure);
        }
    }

    return result;
}

module.exports.loop = function ()
{
    if (getCreepCount() < 5)
    {
        for (var name in Game.spawns)
        {
            var spawn = Game.spawns[name];
            var room = spawn.room;
            var body = findCreepBody(room);

            if (body != undefined)
            {
                var name = createCreepName("H");
                var result = spawn.spawnCreep(body, name);

                if (result != OK)
                {
                    console.log(`Cannot spawn ${name} on ${spawn}`);
                }
                else
                {
                    console.log(`Spawning ${name} : ${body}`);
                }
            }
        }
    }

    var upgraderCount = 0;
    var builderCount = 0;

    for (var name in Game.creeps)
    {
        var creep = Game.creeps[name];
        var room = creep.room;
        var repairables = findRepairables(room);
        var hasConstructions = room.find(FIND_MY_CONSTRUCTION_SITES).length > 0;

        if (creep.store[RESOURCE_ENERGY] == 0)
        {
            creep.memory.harvesting = true;
        }

        if (creep.memory.harvesting)
        {
            if (creep.store.getFreeCapacity(RESOURCE_ENERGY) == 0)
            {
                creep.memory.harvesting = false;
            }
        }

        if (creep.memory.harvesting)
        {
            var sources = room.find(FIND_SOURCES_ACTIVE);

            if (sources.length > 0)
            {
                var source = creep.pos.findClosestByPath(sources);

                if (creep.harvest(source) == ERR_NOT_IN_RANGE)
                {
                    creep.moveTo(source.pos);
                }
            }
        }
        else
        {
            var targets = findEnergyTargets(creep);

            if (targets.length > 0)
            {
                var target = targets[0];

                if (creep.transfer(target, RESOURCE_ENERGY) == ERR_NOT_IN_RANGE)
                {
                    creep.moveTo(target);
                }
            }
            else
            {
                if (repairables.length > 0)
                {
                    var repairable = creep.pos.findClosestByPath(repairables);

                    if (creep.repair(repairable) == ERR_NOT_IN_RANGE)
                    {
                        creep.moveTo(repairable);
                    }
                }
                else if (hasConstructions && (builderCount < upgraderCount))
                {
                    var construction = findConstruction(creep);

                    if (construction != undefined)
                    {
                        if (creep.build(construction) == ERR_NOT_IN_RANGE)
                        {
                            creep.moveTo(construction);
                        }

                        ++builderCount;
                    }
                }
                else
                {
                    if (creep.upgradeController(room.controller) == ERR_NOT_IN_RANGE)
                    {
                        creep.moveTo(room.controller);
                    }

                    ++upgraderCount;
                }
            }
        }
    }
    
}