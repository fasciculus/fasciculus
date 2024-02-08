
function tables(db)
{
    return Object.keys(db);
}

async function types(db)
{
    var table = db['rooms.objects'];
    var entries = await table.find({});
    var types = new Set();

    entries.forEach(e => types.add(e.type));

    return Array.from(types);
}

function controllers(db)
{
    var table = db['rooms.objects'];

    return table.find({type: "controller"});
}

async function roomRCL(table, room)
{
    var controllers = await table.find({type: "controller", room: room});

    if (controllers.length == 0) return 0;

    return controllers[0].level;
}

async function fixSpawn(table, spawn, log)
{
    var room = spawn.room;
    var level = await roomRCL(table, room);

    log(`fixing spawn ${spawn._id} in ${room} with RCL ${level}`);
}

async function fix(db, log)
{
    var table = db['rooms.objects'];
    var spawns = await table.find({type: "spawn"});

    for (var spawn of spawns)
    {
        await fixSpawn(table, spawn, log);
    }
}

module.exports = function(config)
{
    if (config.cli)
    {
        config.cli.on('cliSandbox', function(sandbox)
        {
            sandbox.fasciculus =
            {
                tables: function() { return tables(config.common.storage.db); },
                types: function() { return types(config.common.storage.db); },
                controllers: function() { return controllers(config.common.storage.db); },
                fix: function() { return fix(config.common.storage.db, sandbox.print); }
            };
        });
    }
};