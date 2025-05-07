import { Outlet } from "react-router-dom"
import { BookCart } from "../components/BookCart"
import { useAuth } from "../hooks/useAuth";

export const BooksLayout = () => {
    const { user } = useAuth();

    return (
        <div className="p-8">
            <h1 className="text-center text-3xl font-bold">Welcome {user?.firstName || ""} {user?.lastName || ""}!</h1>
            <p className="text-center text-xl text-slate-500">What would you like to read today?</p>

            <div className="mt-8 grid gap-8 md:gap-0 md:grid-cols-[2fr_1fr]">
                <div className="md:pr-8 md:border-r-2 md:border-r-slate-300 min-w-0">
                    <Outlet />
                </div>

                <div className="order-first md:order-last md:sticky md:top-8 md:h-fit md:pl-8">
                    <BookCart />
                </div>
            </div>
        </div>
    )
}