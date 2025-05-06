import {
    Command,
    CommandEmpty,
    CommandGroup,
    CommandInput,
    CommandItem,
    CommandList,
} from "@/components/ui/command"
import {
    Popover,
    PopoverContent,
    PopoverTrigger,
} from "@/components/ui/popover"
import { useEffect, useState } from "react"
import { Button } from "./ui/button"
import { Check, ChevronsUpDown } from "lucide-react"
import { Category } from "../types/types"
import { cn } from "../lib/utils"
import { useCategories } from "../hooks/useCategories";

interface CategorySelectorProps {
    initialSearchQuery: string;
    value: number | null;
    setValue: (id: number | null) => void;
}

export const CategorySelector = ({ initialSearchQuery = "", value, setValue }: CategorySelectorProps) => {
    const [open, setOpen] = useState(false);
    const [internalValue, setInternalValue] = useState<Category | null>(null);

    const [searchQuery, setSearchQuery] = useState("");
    const { data } = useCategories(1, 10, searchQuery);

    const categories = data?.results || [];

    useEffect(() => {
        setSearchQuery(initialSearchQuery);
    }, [initialSearchQuery]);

    useEffect(() => {
        const category = categories.find(c => c.id === value);
        if (category !== undefined) {
            setInternalValue(category);
        }
        if (value === null) {
            setInternalValue(null)
        }
    }, [value, categories]);

    return (
        <Popover open={open} onOpenChange={setOpen}>
            <PopoverTrigger asChild>
                <Button
                    variant="outline"
                    role="combobox"
                    aria-expanded={open}
                    className="w-[200px] justify-between"
                >
                    {internalValue !== null ? internalValue.name : "Uncategorized"}
                    <ChevronsUpDown className="opacity-50" />
                </Button>
            </PopoverTrigger>
            <PopoverContent className="w-[200px] p-0">
                <Command>
                    <CommandInput placeholder="Search category..." value={searchQuery} onValueChange={setSearchQuery} />
                    <CommandList>
                        <CommandEmpty>No categories found.</CommandEmpty>
                        <CommandGroup>
                            <CommandItem key={0} value={"Uncategorized"} onSelect={() => {
                                setValue(null)
                                setOpen(false)
                            }}>
                                Uncategorized
                            </CommandItem>
                            {categories.map((category) => (
                                <CommandItem
                                    key={category.id}
                                    value={category.name}
                                    onSelect={() => {
                                        setValue(category.id)
                                        setOpen(false)
                                    }}
                                >
                                    {category.name}
                                    <Check
                                        className={cn(
                                            "ml-auto",
                                            internalValue?.id === category.id ? "opacity-100" : "opacity-0"
                                        )}
                                    />
                                </CommandItem>
                            ))}
                        </CommandGroup>
                    </CommandList>
                </Command>
            </PopoverContent>
        </Popover>
    );
}