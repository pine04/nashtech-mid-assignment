import { Label } from "@/components/ui/label";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "./ui/select";

interface PageSizeSelectProps {
    options: number[];
    value: number;
    updateValue: (value: number) => void;
}

export const PageSizeSelect = ({
    options,
    value,
    updateValue
}: PageSizeSelectProps) => {
    return (
        <Label>
            Results per page:
            <Select value={value.toString()} onValueChange={(v) => updateValue(Number(v))}>
                <SelectTrigger>
                    <SelectValue placeholder="Select" />
                </SelectTrigger>
                <SelectContent>
                    {
                        options.map(option =>
                            <SelectItem key={option} value={option.toString()}>
                                {option}
                            </SelectItem>
                        )
                    }
                </SelectContent>
            </Select>
        </Label>
    );
}