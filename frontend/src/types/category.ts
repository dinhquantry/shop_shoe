export interface Category{
    maDm:number;
    tenDm:string;
    moTa?:string|null;
}
export interface CategoryCreateRequest{
    tenDm:string;
    moTa?:string|null;
}