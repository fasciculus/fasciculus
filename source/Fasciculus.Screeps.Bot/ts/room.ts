var _rooms: Room[];

function getRooms(): Room[]
{
    if (!_rooms)
    {
        _rooms = [];

        for (var name in Game.rooms)
        {
            _rooms.push(Game.rooms[name]);
        }
    }

    return _rooms;
}

export const Rooms: Room[] = getRooms();

export class RoomManager
{

}

