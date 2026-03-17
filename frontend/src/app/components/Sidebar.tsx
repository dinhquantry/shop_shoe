"use client";
import Link from "next/link";
import { usePathname } from "next/navigation";
import { LayoutDashboard, Tags, Package, ShoppingCart } from "lucide-react";
import clsx from "clsx";

interface SidebarProps {
  isOpen: boolean;
}

export default function Sidebar({ isOpen }: SidebarProps) {
  const pathname = usePathname();

  const menuItems = [
    { name: "Thống kê", path: "/admin", icon: <LayoutDashboard size={20} /> },
    { name: "Danh mục giày", path: "/admin/categories", icon: <Tags size={20} /> },
    { name: "Quản lý giày", path: "/admin/products", icon: <Package size={20} /> },
    { name: "Đơn hàng", path: "/admin/orders", icon: <ShoppingCart size={20} /> },
  ];

  return (
    <aside
      className={clsx(
        "bg-black shadow-xl transition-all duration-300 ease-in-out flex flex-col z-20",
        isOpen ? "w-64" : "w-20"
      )}
    >
      {/* Logo Area */}
      <div className="h-15 flex items-center justify-center border-b border-gray-100">
        <span className="font-bold text-xl text-blue-600 transition-all whitespace-nowrap overflow-hidden">
          {isOpen ? "HaNa Admin" : "HN"}
        </span>
      </div>

      {/* Navigation */}
      <nav className="flex-1 p-4 space-y-2 overflow-y-auto hidden-scrollbar">
        {menuItems.map((item) => {
          const isActive = pathname === item.path;
          return (
            <Link
              key={item.path}
              href={item.path}
              className={clsx(
                "flex items-center gap-4 px-3 py-3 rounded-lg transition-colors",
                isActive
                  ? "bg-blue-50 text-blue-600 font-medium"
                  : "text-gray-500 hover:bg-gray-100 hover:text-gray-900",
                !isOpen && "justify-center"
              )}
              title={!isOpen ? item.name : ""}
            >
              {item.icon}
              {isOpen && <span className="whitespace-nowrap">{item.name}</span>}
            </Link>
          );
        })}
      </nav>
    </aside>
  );
}