import { ChevronLeft, ChevronRight } from "lucide-react";
import { useState, useEffect } from "react";
import { Button } from "./ui/button";
import { Input } from "./ui/input";

interface PaginationProps {
    currentPageNumber: number;
    maxPageNumber: number;
    goToPreviousPage: () => void;
    goToNextPage: () => void;
    setPageNumber: (pageNumber: number) => void;
}

export const Pagination = ({ currentPageNumber, maxPageNumber, goToPreviousPage, goToNextPage, setPageNumber }: PaginationProps) => {
    const [inputValue, setInputValue] = useState<string>("");

    useEffect(() => {
        setInputValue(currentPageNumber.toString());
    }, [currentPageNumber]);

    const updatePageNumber = () => {
        if (!inputValue) {
            setPageNumber(1);
            setInputValue("1");
            return;
        }

        const pageNumber = parseInt(inputValue);

        if (pageNumber < 1) {
            setPageNumber(1);
            setInputValue("1");
            return;
        }
        if (pageNumber > maxPageNumber) {
            setPageNumber(maxPageNumber);
            setInputValue(maxPageNumber.toString());
            return;
        }
        setPageNumber(pageNumber);
    }

    const handleInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        if (/^\d*$/.test(e.target.value)) {
            setInputValue(e.target.value);
        }
    }

    const handleBlur = () => {
        updatePageNumber();
    }

    const handleKeyDown = (e: React.KeyboardEvent<HTMLInputElement>) => {
        if (e.key !== "Enter") {
            return;
        }

        updatePageNumber();
    }

    return (
        <div className="flex items-center gap-2 text-sm">
            <Button variant="outline" size="icon" onClick={goToPreviousPage} disabled={currentPageNumber === 1}>
                <ChevronLeft />
            </Button>
            <span>Page</span>

            <Input
                type="text"
                value={inputValue}
                onChange={handleInputChange}
                onBlur={handleBlur}
                onKeyDown={handleKeyDown}
                className="w-16"
            />

            <span>/</span>

            <span>{maxPageNumber}</span>

            <Button variant="outline" size="icon" onClick={goToNextPage} disabled={currentPageNumber === maxPageNumber}>
                <ChevronRight />
            </Button>
        </div>
    )
}