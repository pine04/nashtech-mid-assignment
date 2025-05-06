import { Plus } from "lucide-react"
import { Link } from "react-router-dom"
import { Button } from "./ui/button"

export const CategoryAddButton = () => {
    return (
        <Button variant="outline" asChild>
            <Link to="/admin/categories/add">
                Add new <Plus />
            </Link>
        </Button>
    )
}