function setDependableInputFieldToDefault($element, visible, setValue, customValue, dependencyResult) {
    $element.each(function (index, dependentInput) {
        let setSpecialValue = specialValueShouldBeSet($(dependentInput));
        if (isSpecialValue($(dependentInput))) {
            if (visible) {
                unsetSpecialValueIfSelected($(dependentInput));
            } else if (setSpecialValue && dependencyFormulaIsNotSatisfied(dependencyResult)) {
                setMissingValue($(dependentInput), $("#notApplicableId").val(), false);
            }
        } else {
            setInputToDefault(
                $(dependentInput),
                {
                    revertToDefaultEditableState: !setSpecialValue || visible,
                    setValue: setValue,
                    customValue: customValue
                }
            );
        }
    });
}

function updateParentDependencies(updatedParentDependencies) {
    parentDependencies = updatedParentDependencies;
}

function initParentDependencyHandling(parentDependentSelector, parentFieldInstanceRepetiitonId) {
    $(document).on('change time-dependency-change', parentDependentSelector, function (event) {
        executeEventFunctions(event);
        let dependableArguments = parentDependencies[parentFieldInstanceRepetiitonId];
        triggerParentChange(dependableArguments);
    });
}

function triggerParentChange(dependableArguments) {
    let childDependentFieldInstances = {};
    let requestObject = getDependencyRequestObject(dependableArguments, childDependentFieldInstances);

    callServer({
        method: 'POST',
        url: '/FormInstance/ExecuteDependenciesFormulas',
        data: JSON.stringify(requestObject),
        contentType: 'application/json',
        success: function (data) {
            let activePageId = getPreviousActivePageId();

            for (let childFieldInstanceRepetitionId in data.dependenciesResult) {
                let dependencyResult = data.dependenciesResult[childFieldInstanceRepetitionId];

                let dependencyResultHandler = childFieldInstanceShouldBeShown(dependencyResult) ? showChildDependentFieldInstance : hideChildDependentFieldInstance;
                dependencyResultHandler(childDependentFieldInstances, childFieldInstanceRepetitionId, activePageId, dependencyResult);
            }
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr);
        }
    });
}

function getDependencyRequestObject(dependableArguments, childDependentFieldInstances) {
    let depedencyRequestObject = {
        dependencies: []
    };
    $(dependableArguments).each(function (index, dependableArgument) {
        let chieldFieldInstanceRepetitionId = dependableArgument.ChildFieldInstanceRepetitionId;
        let $childDependentInput = getChildDependentInput(dependableArgument.ChildFieldInstanceCssSelector);
        if ($childDependentInput.length > 0) {
            childDependentFieldInstances[chieldFieldInstanceRepetitionId] = $childDependentInput;
            depedencyRequestObject.dependencies.push(getDependencyRequestSingleObject(dependableArgument));
        }
    });

    return depedencyRequestObject;
}

function childFieldInstanceShouldBeShown(dependencyResult) {
    return getPossibleDependencyResult()[dependencyResult.expressionResult];
}

function dependencyFormulaIsNotSatisfied(dependencyResult) {
    return !childFieldInstanceShouldBeShown(dependencyResult);
}

function showChildDependentFieldInstance(childDependentFieldInstances, childFieldInstanceRepetitionId, activePageId, dependencyResult) {
    let $dependentInput = childDependentFieldInstances[childFieldInstanceRepetitionId];
    let $dependentInputElement = $dependentInput.closest('fieldset');

    if (isDependentFieldInstanceShown($dependentInputElement)) {
        return;
    }

    toggleChildDependentField($dependentInputElement, false);
    let customValue = getPreviousValueFromTheMemory($dependentInputElement, $dependentInput, activePageId);
    setPreviousValueToTheMemory($dependentInputElement);
    setTimeout(function () {
        setDependableInputFieldToDefault($dependentInput, true, typeof customValue !== 'undefined', customValue, dependencyResult);
    }, 0);
}

function getPreviousValueFromTheMemory($dependentInputElement, $dependentInput, activePageId) {
    let previousFieldInstanceValue = $dependentInputElement.data("previousFieldInstanceValue");

    let customValue;
    if (fieldInstanceIsOnAnotherPage(activePageId, $dependentInputElement)) {
        if (hasPreviousValueInTheMemory(previousFieldInstanceValue)) {
            $dependentInput.each(function () {
                let flatValues = previousFieldInstanceValue.FlatValues;
                if (hasAnyValue(flatValues) && !isSpecialValue($(this)) && !previousValueWasSpecial(previousFieldInstanceValue)) {
                    customValue = flatValues;
                }
            });
        }
    }

    return customValue;
}

function hideChildDependentFieldInstance(childDependentFieldInstances, childFieldInstanceRepetitionId, activePageId, dependencyResult) {
    let $dependentInput = childDependentFieldInstances[childFieldInstanceRepetitionId];
    let $dependentInputElement = $dependentInput.closest('fieldset');
    let previousFieldInstanceValue = $dependentInputElement.data("previousFieldInstanceValue");

    toggleChildDependentField($dependentInputElement, true);
    if (cleanData(dependencyResult.FieldActions)) {
        if (!hasPreviousValueInTheMemory(previousFieldInstanceValue)) {
            let childFieldInstanceValue = getFormInstanceFieldsSubmissionJson($dependentInputElement)[0];
            setPreviousValueToTheMemory($dependentInputElement, childFieldInstanceValue);
        }
        setTimeout(function () {
            setDependableInputFieldToDefault($dependentInput, false, true, undefined, dependencyResult);
        }, 0);
    }
}

function hasPreviousValueInTheMemory(previousFieldInstanceValue) {
    return previousFieldInstanceValue && !jQuery.isEmptyObject(previousFieldInstanceValue);
}

function previousValueWasSpecial(previousFieldInstanceValue) {
    return previousFieldInstanceValue.IsSpecialValue == 'True';
}

function setPreviousValueToTheMemory($dependentInputElement, childFieldInstanceValue) {
    let newPreviousValue = {};
    if (childFieldInstanceValue) {
        newPreviousValue = {
            FlatValues: childFieldInstanceValue.FlatValues,
            IsSpecialValue: childFieldInstanceValue.IsSpecialValue
        };
    } 
    $dependentInputElement.data("previousFieldInstanceValue", newPreviousValue);
}

function fieldInstanceIsOnAnotherPage(activePageId, $dependentInputElement) {
    return activePageId != getPageId($dependentInputElement);
}

function getPageId($input) {
    return $input.closest('.page').attr('id');
}

function toggleChildDependentField($dependentInputElement, hide) {
    if (hide) {
        $dependentInputElement
            .addClass(getIsHiddenFieldsShown() ? 'show-hidden-fields' : 'd-none')
            .attr('data-dependables', 'False')
            .attr('disabled', 'disabled');
    } else {
        $dependentInputElement
            .removeClass(getIsHiddenFieldsShown() ? 'show-hidden-fields' : 'd-none')
            .attr('data-dependables', 'True')
            .removeAttr('disabled');
    }
}

function cleanData(fieldActions) {
    return fieldActions?.some(fA => fA == 0) ?? false;
}

function getDependencyRequestSingleObject(dependableArgument) {
    let variableFieldInstanceValues = getVariableFieldInstanceValues(dependableArgument);
    let dependencyObject = { ...dependableArgument };
    dependencyObject.fieldInstancesInFormula = variableFieldInstanceValues;
    return dependencyObject;
}

function getChildDependentInput(selector) {
    return $(`${selector}`);
}

function getVariableFieldInstanceValues(dependableArgument) {
    let fieldIds = dependableArgument.DependentOnFieldInfos.map(x => x.FieldId);
    let querySelector = fieldIds.map(fieldId => `[data-fieldid='${fieldId}']`).join(", ");
    let filteredFieldInstanceElements = $(querySelector).closest(getFormInstanceFieldsSelector());
    return getFormInstanceFieldsSubmissionJson(filteredFieldInstanceElements);
}

function getPossibleDependencyResult() {
    return {
        0: true,
        1: false,
        2: undefined
    }
}

function isDependentFieldInstanceHidden(input) {
    let $dependentInputElement = $(input).closest('fieldset');
    return $dependentInputElement.attr('data-dependables') === 'False';
}

function isDependentFieldInstanceShown($dependentInputElement) {
    return $dependentInputElement.attr('data-dependables') === 'True';
}