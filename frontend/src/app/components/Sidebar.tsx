"use client";
import Link from "next/link";
import { usePathname } from "next/navigation";
import { LayoutDashboard, Tags, Package, ShoppingCart, Brackets } from "lucide-react";

export default function Sidebar({ isOpen }: { isOpen: boolean }) {
  const pathname = usePathname();

  const menuItems = [
    { name: "Thống kê", path: "/admin", icon: <LayoutDashboard /> },
    { name: "Danh mục", path: "/admin/categories", icon: <Tags /> },
    { name: "Sản phẩm", path: "/admin/products", icon: <Package /> },
    { name: "Thương hiệu", path: "/admin/brands", icon: <Brackets/> },
    { name: "Đơn hàng", path: "/admin/orders", icon: <ShoppingCart /> },
  ];

  return (
    <aside className={`bg-gray-900 text-white flex flex-col ${isOpen ? "w-64" : "w-16"}`}>
      {/* Logo */}
      <div className="text-blue-700 h-16 flex items-center justify-center border-b border-gray-800 font-bold text-lg">
        {isOpen ? "HANA ADMIN" : "HN"}
      </div>

      {/* Nav */}
      <nav className="p-2 space-y-1">
        {menuItems.map((item) => {
          const isActive = pathname === item.path;
          return (
            <Link
              key={item.path}
              href={item.path}
              className={`flex items-center gap-4 p-3 rounded-md transition ${
                isActive ? "bg-blue-600" : "hover:bg-gray-800 text-gray-400"
              } ${!isOpen && "justify-center"}`}
            >
              {item.icon}
              {isOpen && <span className="text-sm">{item.name}</span>}
            </Link>
          );
        })}
      </nav>
    </aside>
  );
}