import { Component, Input, ViewEncapsulation } from "@angular/core";

@Component({
    selector: "quiz-search",
    templateUrl: './quiz-search.component.html',
    styleUrls: ['./quiz-search.component.css'],
    encapsulation: ViewEncapsulation.None,
})

export class QuizSearchComponent {
    @Input() class: string;
    @Input() placeholder: string;
}