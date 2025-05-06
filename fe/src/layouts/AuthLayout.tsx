import { Outlet } from "react-router-dom"
import { Toaster } from "sonner"
import { Footer } from "@/components/Footer"

export const AuthLayout = () => {
    return (
        <div className="min-h-screen grid grid-rows-[1fr_auto]">
            <main>
                <Outlet />
            </main>
            <Toaster />
            <Footer />
        </div>
    )
}