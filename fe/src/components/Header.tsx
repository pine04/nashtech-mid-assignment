import { Menu, LogOut } from "lucide-react";
import { ReactNode, useEffect, useState } from "react";
import { Link } from "react-router-dom";
import { cn } from "../lib/utils";
import { Button } from "./ui/button";
import { useAuth } from "../hooks/useAuth";

interface HeaderLinkProps {
    to: string;
    onClick: () => void;
    children: ReactNode;
}

const HeaderLink = ({ to, onClick, children }: HeaderLinkProps) => {
    return (
        <Link to={to} onClick={onClick} className="relative content-[''] after:w-0 after:h-0.5 after:absolute after:top-full after:left-0 after:bg-white hover:after:w-full after:origin-left after:transition-all">
            {children}
        </Link>
    )
}

const normalUserLinks = [
    { name: "Books", path: "/books" },
    { name: "My Borrowings", path: "/my-borrowings" },
    { name: "Profile", path: "/profile" },
];

const superUserLinks = [
    { name: "Requests", path: "/admin/requests" },
    { name: "Books", path: "/admin/books" },
    { name: "Categories", path: "/admin/categories" },
    { name: "Profile", path: "/admin/profile" },
]

export const Header = () => {
    const { user, logoutMutation } = useAuth();
    const [collapsed, setCollapsed] = useState(true);

    const links = (user && user.role === "SuperUser") ? superUserLinks : normalUserLinks;

    const collapse = () => setCollapsed(true);
    const uncollapse = () => setCollapsed(false);

    useEffect(() => {
        window.addEventListener("resize", collapse);

        return () => window.removeEventListener("resize", collapse);
    }, []);

    const handleLogout = async () => {
        await logoutMutation.mutateAsync();
    }

    return (
        <header className="px-4 sm:px-9 py-4 bg-blue-800 text-white font-bold flex justify-between items-center">
            <p className="text-xl sm:text-2xl uppercase text-nowrap">Library Management</p>

            <Button variant="ghost" className="size-10 md:hidden" onClick={uncollapse}>
                <Menu className="size-8" />
            </Button>

            <div id="overlay" className={cn("fixed top-0 left-0 w-screen h-screen bg-blue-200/25 z-40 md:hidden", collapsed && "hidden")} onClick={collapse}></div>

            <nav className={cn("fixed top-0 right-0 w-64 h-screen z-50 overflow-clip md:static md:w-fit md:h-fit md:overflow-visible md:pointer-events-auto", collapsed && "pointer-events-none", !collapsed && "pointer-events-auto")}>
                <ul className={cn("absolute top-0 w-full h-full bg-blue-800 px-8 py-12 text-2xl flex flex-col transition-all md:static md:p-0 md:w-fit md:h-fit md:text-base md:flex-row gap-9 md:items-center", collapsed && "left-full", !collapsed && "left-0")}>
                    {
                        links.map(link =>
                            <li key={link.path}>
                                <HeaderLink to={link.path} onClick={collapse}>{link.name}</HeaderLink>
                            </li>
                        )
                    }
                    <li>
                        <Button variant="ghost" className="size-10 md:size-9" onClick={handleLogout}>
                            <LogOut className="size-8 md:size-4" />
                        </Button>
                    </li>
                </ul>
            </nav>
        </header>
    );
}

