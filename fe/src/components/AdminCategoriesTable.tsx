import { Edit, Trash } from "lucide-react";
import { Link } from "react-router-dom";
import { Category } from "../types/types";
import { Button, buttonVariants } from "./ui/button";
import { Table, TableHeader, TableRow, TableHead, TableBody, TableCell } from "./ui/table";
import {
    AlertDialog,
    AlertDialogAction,
    AlertDialogCancel,
    AlertDialogContent,
    AlertDialogDescription,
    AlertDialogFooter,
    AlertDialogHeader,
    AlertDialogTitle,
    AlertDialogTrigger,
} from "@/components/ui/alert-dialog";
import { useDeleteCategory } from "../hooks/useDeleteCategory";

interface CategoryDeleteButtonProps {
    categoryId: number;
}

const CategoryDeleteButton = ({ categoryId }: CategoryDeleteButtonProps) => {
    const { deleteCategory } = useDeleteCategory(categoryId);

    const handleDelete = () => {
        deleteCategory();
    }

    return (
        <AlertDialog>
            <AlertDialogTrigger asChild>
                <Button variant="outline" size="icon">
                    <Trash />
                </Button>
            </AlertDialogTrigger>
            <AlertDialogContent>
                <AlertDialogHeader>
                    <AlertDialogTitle>Are you sure?</AlertDialogTitle>
                    <AlertDialogDescription>
                        This action cannot be undone.
                    </AlertDialogDescription>
                </AlertDialogHeader>
                <AlertDialogFooter>
                    <AlertDialogCancel>Cancel</AlertDialogCancel>
                    <AlertDialogAction className={buttonVariants({ variant: "destructive" })} onClick={handleDelete}>Continue</AlertDialogAction>
                </AlertDialogFooter>
            </AlertDialogContent>
        </AlertDialog>
    )
}

interface CategoryEditButtonProps {
    categoryId: number;
}

const CategoryEditButton = ({ categoryId }: CategoryEditButtonProps) => {
    return (
        <Button asChild variant="outline" size="icon">
            <Link to={`/admin/categories/${categoryId}/edit`}>
                <Edit />
            </Link>
        </Button>
    )
}

interface AdminCategoriesTableProps {
    categories: Category[] | undefined;
    isLoading: boolean;
    isError: boolean;
}

export const AdminCategoriesTable = ({ categories, isLoading, isError }: AdminCategoriesTableProps) => {
    return (
        <Table>
            <TableHeader className="text-lg font-bold">
                <TableRow>
                    <TableHead className="text-center">ID</TableHead>
                    <TableHead className="text-center w-48">Name</TableHead>
                    <TableHead className="text-center">Book Count</TableHead>
                    <TableHead className="text-center">Action</TableHead>
                </TableRow>
            </TableHeader>
            <TableBody>
                {
                    isLoading &&
                    <TableRow>
                        <TableCell colSpan={6} className="text-3xl text-center py-4">Loading...</TableCell>
                    </TableRow>
                }
                {
                    isError &&
                    <TableRow>
                        <TableCell colSpan={6} className="text-3xl text-center py-4">Error getting categories. Please try again later.</TableCell>
                    </TableRow>
                }
                {
                    categories &&
                    categories.map(category =>
                        <TableRow key={category.id}>
                            <TableCell className="text-center">{category.id}</TableCell>
                            <TableCell>
                                <p className="w-48 line-clamp-2 text-wrap">{category.name}</p>
                            </TableCell>
                            <TableCell className="text-center">{category.bookCount}</TableCell>
                            <TableCell className="flex justify-center gap-4">
                                <CategoryEditButton categoryId={category.id} />
                                <CategoryDeleteButton categoryId={category.id} />
                            </TableCell>
                        </TableRow>
                    )
                }
            </TableBody>
        </Table>
    )
}