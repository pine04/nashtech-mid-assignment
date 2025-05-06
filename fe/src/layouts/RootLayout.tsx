import { Outlet } from "react-router-dom"
import { Toaster } from "sonner"
import { Footer } from "@/components/Footer"
import { Header } from "@/components/Header"

export const Layout = () => {
    return (
        <div className="min-h-screen grid grid-rows-[auto_1fr_auto]">
            <Header />
            <main>
                <Outlet />
            </main>
            <Toaster />
            <Footer />
        </div>
    )
}