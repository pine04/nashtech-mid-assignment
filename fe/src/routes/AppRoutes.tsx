import { Routes, Route, Navigate } from "react-router-dom"
import { BookDetailsPage } from "@/pages/BookDetailsPage"
import { BooksPage } from "@/pages/BooksPage"
import { LoginPage } from "../pages/LoginPage"
import { RegisterPage } from "../pages/RegisterPage"
import { ProfilePage } from "../pages/ProfilePage"
import { NormalUserRoute } from "./NormalUserRoute"
import { AuthRoute } from "./AuthRoute"
import { MyBorrowingsPage } from "../pages/MyBorrowingsPage"
import { SuperUserRoute } from "./SuperUserRoute"
import { AdminBooksPage } from "../pages/AdminBooksPage"
import { BookEditPage } from "../pages/BookEditPage"
import { BookAddPage } from "../pages/BookAddPage"
import { AdminCategoriesPage } from "../pages/AdminCategoriesPage"
import { CategoryAddPage } from "../pages/CategoryAddPage"
import { CategoryEditPage } from "../pages/CategoryEditPage"
import { AdminRequestsPage } from "../pages/AdminRequestsPage"

export const AppRoutes = () => {
    return (
        <Routes>
            <Route path="/" element={<NormalUserRoute />}>
                <Route path="" element={<Navigate to="/books" />} />

                <Route path="books" element={<BooksPage />} />
                <Route path="books/:id" element={<BookDetailsPage />} />

                <Route path="my-borrowings" element={<MyBorrowingsPage />} />

                <Route path="profile" element={<ProfilePage />} />
            </Route>

            <Route path="/admin" element={<SuperUserRoute />}>
                <Route path="" element={<Navigate to="/admin/requests" />} />

                <Route path="books" element={<AdminBooksPage />} />
                <Route path="books/:id/edit" element={<BookEditPage />} />
                <Route path="books/add" element={<BookAddPage />} />

                <Route path="categories" element={<AdminCategoriesPage />} />
                <Route path="categories/:id/edit" element={<CategoryEditPage />} />
                <Route path="categories/add" element={<CategoryAddPage />} />

                <Route path="requests" element={<AdminRequestsPage />} />
                <Route path="profile" element={<ProfilePage />} />
            </Route>


            <Route path="/" element={<AuthRoute />}>
                <Route path="login" element={<LoginPage />} />
                <Route path="register" element={<RegisterPage />} />
            </Route>
        </Routes>
    )
}