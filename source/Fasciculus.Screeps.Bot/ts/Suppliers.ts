import { Bodies, BodyTemplate } from "./Bodies";
import { CreepBase, Creeps } from "./Creeps";
import { Extensions } from "./Extensions";
import { GameWrap } from "./GameWrap";
import { CreepBaseMemory } from "./Memories";
import { Spawns } from "./Spawns";
import { Customer, CustomerId, SupplyId, Supply, CreepState, CreepType } from "./Types";
import { Stores } from "./Stores";
import { Dictionaries, Dictionary, Vector } from "./Collections";
import { Positions } from "./Positions";
import { profile } from "./Profiling";

const MIN_SUPPLY_ENERGY = 10;
const SUPPLIER_PERFORMANCE_FACTOR = 1.25;

const SUPPLIER_TEMPLATE: BodyTemplate = BodyTemplate.create([CARRY, MOVE, CARRY, MOVE]).add([CARRY, MOVE, CARRY, MOVE]).add([CARRY, MOVE], 21);

const SUPPLIER_MOVE_TO_OPTS: MoveToOpts =
{
    reusePath: 6,
    ignoreCreeps: false,

    visualizePathStyle:
    {
        stroke: "#ff0"
    }
};

interface SupplierMemory extends CreepBaseMemory
{
    customer?: CustomerId;
    supply?: SupplyId;

    travelled?: number;
    handled?: number;
}

export class Supplier extends CreepBase<SupplierMemory>
{
    private _supply?: Supply;
    private _customer?: Customer;

    private _travelled: number;
    private _handled: number;

    get supply(): Supply | undefined { return this._supply; }
    set supply(value: Supply | undefined) { this._supply = value; this.memory.supply = value?.id; }

    get customer(): Customer | undefined { return this._customer; }
    set customer(value: Customer | undefined) { this._customer = value; this.memory.customer = value?.id; }

    get travelled(): number { return this._travelled; }
    private incrementTravelled() { this._travelled = this.memory.travelled = this._travelled + 1; }

    get handled(): number { return this._handled; }
    private addHandled(amount: number) { this._handled = this.memory.handled = this._handled + amount; }

    get performance(): number { return (this.handled / this.travelled) * SUPPLIER_PERFORMANCE_FACTOR; }

    constructor(creep: Creep)
    {
        super(creep);

        let memory = this.memory;

        this.supply = GameWrap.get(memory.supply);
        this._customer = GameWrap.get(memory.customer);

        this._travelled = memory.travelled || 1;
        this._handled = memory.handled || this.energyCapacity;
    }

    execute()
    {
        switch (this.state)
        {
            case CreepState.ToSupply: this.executeToSupply(); break;
            case CreepState.ToCustomer: this.executeToCustomer(); break;
            case CreepState.Withdraw: this.executeWithdraw(); break;
            case CreepState.Transfer: this.executeTransfer(); break;
        }
    }

    private executeToSupply()
    {
        this.incrementTravelled();

        let supply = this.supply;

        if (!supply) return;

        this.moveTo(supply, SUPPLIER_MOVE_TO_OPTS);
    }

    private executeToCustomer()
    {
        this.incrementTravelled();

        let customer = this.customer;

        if (!customer) return;

        this.moveTo(customer, SUPPLIER_MOVE_TO_OPTS);
    }

    @profile
    private executeWithdraw()
    {
        let supply = this.supply;

        if (!supply) return;

        let amount = Math.min(this.freeEnergyCapacity, Stores.energy(supply));

        this.withdraw(supply, RESOURCE_ENERGY, amount);
        this.onHandled(amount);
    }

    @profile
    private executeTransfer()
    {
        let customer = this.customer;

        if (!customer) return;

        let amount: number = Math.min(this.energy, Stores.freeEnergyCapacity(customer));

        this.transfer(customer, RESOURCE_ENERGY, amount);

        if (customer instanceof Creep) this.customer = undefined;

        this.onHandled(amount);
    }

    private onHandled(amount: number)
    {
        this.addHandled(amount);

        if (this._travelled > 100)
        {
            let memory = this.memory;

            this._travelled = memory.travelled = this._travelled / 2;
            this._handled = memory.handled = this._handled / 2;
        }
    }

    prepare()
    {
        switch (this.state)
        {
            case CreepState.Idle: this.state = this.prepareIdle();
            case CreepState.ToSupply: this.state = this.prepareMoveToSupply();
            case CreepState.ToCustomer: this.state =  this.prepareMoveToCustomer();
            case CreepState.Withdraw: this.state =  this.prepareWithdraw();
            case CreepState.Transfer: this.state = this.prepareTransfer();
        }
    }

    @profile
    private prepareIdle(): CreepState
    {
        let supply: Supply | undefined = this.supply;
        let customer: Customer | undefined = this.customer;

        if (customer && !Supplier.hasCapacity(customer))
        {
            this.customer = customer = undefined;
        }

        if (customer && this.energy == 0)
        {
            this.customer = customer = undefined;
        }

        if (customer)
        {
            return this.inRangeTo(customer) ? CreepState.Transfer : CreepState.ToCustomer;
        }

        if (supply && this.freeEnergyCapacity == 0)
        {
            this.supply = supply = undefined;
        }

        if (supply && !Supplier.hasEnergy(supply))
        {
            this.supply = supply = undefined;
        }

        if (supply)
        {
            return this.inRangeTo(supply) ? CreepState.Withdraw : CreepState.ToSupply;
        }

        return CreepState.Idle;
    }

    @profile
    private prepareMoveToSupply(): CreepState
    {
        let supply = this.supply;

        if (!supply) return this.prepareIdle();
        if (this.freeEnergyCapacity == 0) return this.prepareIdle();
        if (!Supplier.hasEnergy(supply)) return this.prepareIdle();

        return this.inRangeTo(supply) ? CreepState.Withdraw : CreepState.ToSupply;
    }

    @profile
    private prepareMoveToCustomer(): CreepState
    {
        let customer = this.customer;

        if (!customer) return this.prepareIdle();
        if (this.energy == 0) return this.prepareIdle();
        if (!Supplier.hasCapacity(customer)) return this.prepareIdle();

        return this.inRangeTo(customer) ? CreepState.Transfer : CreepState.ToCustomer;
    }

    @profile
    private prepareWithdraw(): CreepState
    {
        let supply = this.supply;

        if (!supply) return this.prepareIdle();
        if (this.freeEnergyCapacity == 0) return this.prepareIdle();
        if (!Supplier.hasEnergy(supply)) return this.prepareIdle();

        return this.inRangeTo(supply) ? CreepState.Withdraw : CreepState.ToSupply;
    }

    @profile
    private prepareTransfer(): CreepState
    {
        let customer = this.customer;

        if (!customer) return this.prepareIdle();
        if (this.energy == 0) return this.prepareIdle();
        if (!Supplier.hasCapacity(customer)) return this.prepareIdle();

        return this.inRangeTo(customer) ? CreepState.Transfer : CreepState.ToCustomer;
    }

    private inRangeTo(target: Supply | Customer): boolean
    {
        return this.pos.inRangeTo(target, 1);
    }

    static hasEnergy(supply: Supply): boolean
    {
        return Stores.energy(supply) >= MIN_SUPPLY_ENERGY;
    }

    static hasCapacity(customer: Customer): boolean
    {
        let maxRatio = customer instanceof Creep ? 0.8 : 1;

        return Stores.energyRatio(customer) < maxRatio;
    }
}

interface SupplierInfo
{
    supplier: Supplier;
    name: string;
    pos: RoomPosition;
    energy: number;
    capacity: number;
    supply: Supply | undefined;
    customer: Customer | undefined;
}

interface SupplyInfo
{
    supply: Supply;
    id: SupplyId;
    energy: number;
    capacity: number;
}

interface CustomerInfo
{
    customer: Customer;
    id: CustomerId;
    priority: number;
    demand: number;
    ratio: number;
}

export class Suppliers
{
    private static _all: Vector<Supplier> = new Vector();

    static get count(): number { return Suppliers._all.length; }
    static get idleCount(): number { return Suppliers._all.filter(s => s.state == CreepState.Idle).length; }

    static get performance(): number { return Suppliers._all.sum(s => s.performance); }

    static initialize()
    {
        Suppliers._all = Creeps.ofType(CreepType.Supplier).map(c => new Supplier(c));

        Bodies.register(CreepType.Supplier, SUPPLIER_TEMPLATE);
    }

    static run()
    {
        Suppliers.prepare(Suppliers._all);
        Suppliers.prepare(Suppliers.assign());
        Suppliers.execute(Suppliers._all);
    }

    private static prepare(suppliers: Vector<Supplier>)
    {
        suppliers.forEach(s => s.prepare());
    }

    // @profile
    private static execute(suppliers: Vector<Supplier>)
    {
        suppliers.forEach(s => s.execute());
    }

    private static assign(): Vector<Supplier>
    {
        var result: Vector<Supplier> = new Vector();
        var unassigned: Dictionary<SupplierInfo> = Suppliers.findUnassignedSuppliers();

        if (Dictionaries.isEmpty(unassigned)) return result;

        result = Suppliers.assignCustomers(unassigned);

        if (Dictionaries.isEmpty(unassigned)) return result;

        return result.concat(Suppliers.assignSupplies(unassigned));
    }

    @profile
    private static assignCustomers(unassigned: Dictionary<SupplierInfo>): Vector<Supplier>
    {
        let result: Vector<Supplier> = new Vector();
        let suppliers: Dictionary<SupplierInfo> = Suppliers.findSuppliersWithEnergy(unassigned);

        if (Dictionaries.isEmpty(suppliers)) return result;

        let customers: Dictionary<CustomerInfo> = Suppliers.findCustomers();

        if (Dictionaries.isEmpty(customers)) return result;

        let sorted: Vector<CustomerInfo> = Suppliers.sortCustomers(customers);

        for (let customerInfo of sorted)
        {
            while (customerInfo.demand > 0)
            {
                let customer: Customer = customerInfo.customer;
                let supplierInfo: SupplierInfo | undefined = Suppliers.findNearest(customer, suppliers);

                if (!supplierInfo) break;

                let supplier: Supplier = supplierInfo.supplier;

                supplier.customer = customer;
                customerInfo.demand -= supplierInfo.energy;

                delete suppliers[supplierInfo.name];
                delete unassigned[supplierInfo.name];

                result.append(supplier);
            }

            if (Dictionaries.isEmpty(suppliers)) break;
        }

        return result;
    }

    @profile
    private static assignSupplies(suppliers: Dictionary<SupplierInfo>): Vector<Supplier>
    {
        let result: Vector<Supplier> = new Vector();
        let supplies: Dictionary<SupplyInfo> = Suppliers.findSupplies();
        let sorted: Vector<SupplyInfo> = Suppliers.sortSupplies(supplies);

        for (let supplyInfo of sorted)
        {
            while (supplyInfo.energy > MIN_SUPPLY_ENERGY)
            {
                let supply: Supply = supplyInfo.supply;
                let supplierInfo: SupplierInfo | undefined = Suppliers.findNearest(supply, suppliers);

                if (!supplierInfo) break;

                let supplier: Supplier = supplierInfo.supplier;

                supplier.supply = supply;
                supplyInfo.energy -= supplierInfo.capacity;
                delete suppliers[supplierInfo.name];
                result.append(supplier);
            }

            if (Dictionaries.isEmpty(suppliers)) break;
        }

        return result;
    }

    @profile
    private static findSuppliersWithEnergy(unassigned: Dictionary<SupplierInfo>): Dictionary<SupplierInfo>
    {
        return Dictionaries.values(unassigned).filter(s => s.energy > 0).indexBy(s => s.name);
    }

    private static findCustomers(): Dictionary<CustomerInfo>
    {
        let spawns: Vector<Customer> = Spawns.my.map(s => s.spawn);
        let extensions: Vector<Customer> = Extensions.my;
        let upgraders: Vector<Customer> = Creeps.ofType(CreepType.Upgrader);
        let builders: Vector<Customer> = Creeps.ofType(CreepType.Builder);
        let repairers: Vector<Customer> = Creeps.ofType(CreepType.Repairer);
        var customers: Vector<Customer> = spawns.concat(extensions).concat(upgraders).concat(builders).concat(repairers);

        customers = customers.filter(c => Supplier.hasCapacity(c));

        let infos: Vector<CustomerInfo> = customers.map(Suppliers.createCustomerInfo);
        let result: Dictionary<CustomerInfo> = infos.indexBy(i => i.customer.id);

        Suppliers.adjustCustomerDemands(result);
        Suppliers.filterCustomers(result);

        return result;
    }

    private static findSupplies(): Dictionary<SupplyInfo>
    {
        let wellers: Vector<Supply> = Creeps.ofType(CreepType.Weller);
        let supplies: Vector<Supply> = wellers;

        supplies = supplies.filter(s => Supplier.hasEnergy(s));

        let infos: Vector<SupplyInfo> = supplies.map(Suppliers.createSupplyInfo);
        let result: Dictionary<SupplyInfo> = infos.indexBy(i => i.supply.id);

        Suppliers.adjustSupplyEnergies(result);
        Suppliers.filterSupplies(result);

        return result;
    }

    @profile
    private static adjustCustomerDemands(infos: Dictionary<CustomerInfo>)
    {
        for (let supplier of Suppliers._all)
        {
            let customer = supplier.customer;

            if (!customer) continue;

            let info = infos[customer.id];

            if (!info) continue;

            info.demand -= supplier.energy;
        }
    }

    private static adjustSupplyEnergies(infos: Dictionary<SupplyInfo>)
    {
        for (let supplier of Suppliers._all)
        {
            let supply = supplier.supply;

            if (!supply) continue;

            let info = infos[supply.id];

            if (!info) continue;

            info.energy -= supplier.freeEnergyCapacity;
        }
    }

    private static filterCustomers(infos: Dictionary<CustomerInfo>)
    {
        let serveds = Dictionaries.values(infos).filter(i => i.demand <= 0);

        for (let served of serveds)
        {
            delete infos[served.id];
        }
    }

    private static filterSupplies(infos: Dictionary<SupplyInfo>)
    {
        let useds = Dictionaries.values<SupplyInfo>(infos).filter(i => i.energy <= 0);

        for (let used of useds)
        {
            delete infos[used.id];
        }
    }

    private static findUnassignedSuppliers(): Dictionary<SupplierInfo>
    {
        var infos: Vector<SupplierInfo> = Suppliers._all.map(Suppliers.createSupplierInfo);

        infos = infos.filter(i => !i.supply && !i.customer);

        return infos.indexBy(i => i.name);
    }

    private static createSupplierInfo(supplier: Supplier): SupplierInfo
    {
        let info: SupplierInfo =
        {
            supplier: supplier,
            name: supplier.name,
            pos: supplier.pos,
            energy: supplier.energy,
            capacity: supplier.freeEnergyCapacity,
            supply: supplier.supply,
            customer: supplier.customer
        }

        return info;
    }

    private static createSupplyInfo(supply: Supply): SupplyInfo
    {
        let info: SupplyInfo =
        {
            supply: supply,
            id: supply.id,
            energy: Stores.energy(supply),
            capacity:Stores.freeEnergyCapacity(supply)
        };

        return info;
    }

    private static createCustomerInfo(customer: Customer): CustomerInfo
    {
        let info: CustomerInfo =
        {
            customer: customer,
            id: customer.id,
            priority: Suppliers.customerPriority(customer),
            demand: Stores.freeEnergyCapacity(customer),
            ratio: Stores.energyRatio(customer)
        }

        return info;
    }

    private static customerPriority(customer: Customer): number
    {
        if (customer instanceof StructureSpawn) return 1;
        if (customer instanceof StructureExtension) return 2;

        return 3;
    }

    private static sortSupplies(supplies: Dictionary<SupplyInfo>): Vector<SupplyInfo>
    {
        return Dictionaries.values(supplies).sort(Suppliers.compareSupplies);
    }

    private static sortCustomers(customers: Dictionary<CustomerInfo>): Vector<CustomerInfo>
    {
        return Dictionaries.values(customers).sort(Suppliers.compareCustomers);
    }

    private static compareCustomers(a: CustomerInfo, b: CustomerInfo): number
    {
        let result = a.priority - b.priority;

        if (result != 0) return result;

        return b.ratio - b.ratio;
    }

    private static compareSupplies(a: SupplyInfo, b: SupplyInfo): number
    {
        let result = a.capacity - b.capacity;

        if (result != 0) return result;

        return b.energy - a.energy;
    }

    @profile
    private static findNearest(target: Customer | Supply, suppliers: Dictionary<SupplierInfo>): SupplierInfo | undefined
    {
        return Positions.closestByRange(target, Dictionaries.values(suppliers));
    }
}