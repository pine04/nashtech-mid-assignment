import { Plus } from "lucide-react"
import { Link } from "react-router-dom"
import { Button } from "./ui/button"

export const BookAddButton = () => {
    return (
        <Button variant="outline" asChild>
            <Link to="/admin/books/add">
                Add new <Plus />
            </Link>
        </Button>
    )
}