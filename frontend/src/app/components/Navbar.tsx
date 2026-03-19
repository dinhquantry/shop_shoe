"use client";

import { Menu } from "lucide-react";

export default function Navbar({ toggleSidebar }: { toggleSidebar: () => void }) {
  return (
    <header className="flex items-center justify-between h-16 px-6 bg-white shadow-sm">
      {/* Nút Toggle Sidebar */}
      <button onClick={toggleSidebar} className="text-gray-600 hover:text-black">
        <Menu />
      </button>

      {/* User Menu */}
      <div className="flex items-center gap-2">
        <div className="flex items-center justify-center w-8 h-8 font-bold text-white bg-blue-600 rounded-full">
          A
        </div>
        <span className="text-sm font-medium">Admin</span>
      </div>
    </header>
  );
}