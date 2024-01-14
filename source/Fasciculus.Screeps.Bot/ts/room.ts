
export function getRooms(): Room[]
{
    var result: Room[] = [];

    for (let name in Game.rooms)
    {
        result.push(Game.rooms[name]);
    }

    return result;
}