
export type ContainerId = Id<StructureContainer>;
export type ControllerId = Id<StructureController>;
export type SiteId = Id<ConstructionSite>;
export type SourceId = Id<Source>;

export type Supply = Creep | StructureContainer;
export type SupplyId = Id<Creep | StructureContainer>;

export type CustomerId = Id<Creep | StructureSpawn | StructureExtension>;
export type Customer = Creep | StructureSpawn | StructureExtension;

export type RepairableId = Id<StructureRoad | StructureWall>
export type Repairable = StructureRoad | StructureWall
